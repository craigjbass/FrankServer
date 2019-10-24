using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using FluentAssertions;
using Frank.API.WebDevelopers;
using Frank.API.WebDevelopers.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using static Frank.API.WebDevelopers.DTO.ResponseBuilders;

namespace Frank.EndToEndTests
{
    public class FrankTest
    {
        private IWebApplicationBuilder _builder;
        private IWebApplication _webApplication;
        private IRestResponse _response;

        private void MakeGetRequest(string path)
        {
            var request = new RestRequest(path, Method.GET);
            var client = new RestClient("http://127.0.0.1:8019/");

            _response = client.Execute(request);
        }

        private IRestResponse TheResponse()
        {
            if (_response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new Exception(
                    Encoding.UTF8.GetString(_response.RawBytes)
                );
            }

            return _response;
        }

        private JObject TheResponseBody()
        {
            return JsonConvert.DeserializeObject<JObject>(TheResponse().Content);
        }

        [SetUp]
        public void SetUp()
        {
            _builder = System.Frank.Configure();
            _builder.ListenOn("127.0.0.1", "8019");
        }

        [TearDown]
        public void TearDown()
        {
            StopFrank();
        }

        private void StartFrank()
        {
            _webApplication = _builder.Build();
            _webApplication.Start();
        }

        private void StartFrankWithRoutes(Action<IRouteConfigurer> action)
        {
            _builder.WithRoutes(action);
            StartFrank();
        }

        private void StopFrank()
        {
            _webApplication.Stop();
        }
        
        [Test]
        public void CanServeNotFound()
        {
            StartFrank();

            MakeGetRequest("/");
            TheResponse().StatusCode.Should().Be(404);
        }

        [Test]
        public void CanServeNotFoundTwice()
        {
            StartFrank();

            MakeGetRequest("/");
            TheResponse().StatusCode.Should().Be(404);

            MakeGetRequest("/another-route");
            TheResponse().StatusCode.Should().Be(404);
        }

        [TestCase("/success")]
        [TestCase("/okay")]
        public void CanServeOk(string route)
        {
            StartFrankWithRoutes(router => { router.Get(route).To(Ok); });
            MakeGetRequest(route);
            TheResponse().StatusCode.Should().Be(200);
        }

        [Test]
        public void CanServeACreatedStatusCode()
        {
            StartFrankWithRoutes(router => { router.Get("/created").To(Created); });
            MakeGetRequest("/created");
            TheResponse().StatusCode.Should().Be(201);
        }

        [Test]
        public void CanSerializeResponseIntoHttpResponse()
        {
            StartFrankWithRoutes(
                router =>
                {
                    router.Get("/foo/2")
                        .To(() => Ok().WithBody(new {Id = 2}));
                }
            );


            MakeGetRequest("/foo/2");

            TheResponse().StatusCode.Should().Be(200);
            TheResponseBody()["Id"].Value<int>().Should().Be(2);
        }

        [Test]
        public void CanDeserializeIncomingGetRequest()
        {
            Request? processedRequest = null;
            StartFrankWithRoutes(
                router =>
                {
                    router.Get("/foo/2")
                        .To(request =>
                            {
                                processedRequest = request;
                                return Ok();
                            }
                        );
                }
            );

            new RestClient("http://127.0.0.1:8019/").Execute(
                new RestRequest("/foo/2", Method.GET)
                    .AddParameter("bar", "123")
                    .AddHeader("X-Api-Key", "1234supersecure")
            );

            processedRequest.Should().NotBeNull();
            processedRequest?.Path.Should().Be("/foo/2");
            processedRequest?.QueryParameters["bar"].Should().Be("123");
            processedRequest?.Body.Should().Be("");
            processedRequest?.Headers["x-api-key"].Should().Be("1234supersecure");
        }

        [Test]
        public void CanDeserializeIncomingPostRequest()
        {
            Request? processedRequest = null;
            StartFrankWithRoutes(
                router =>
                {
                    router.Post("/foo/2")
                        .To(request =>
                            {
                                processedRequest = request;
                                return Ok();
                            }
                        );
                }
            );

            new RestClient("http://127.0.0.1:8019/").Execute(
                new RestRequest("/foo/2", Method.POST)
                    .AddQueryParameter("bar", "123")
                    .AddHeader("X-Api-Key", "1234supersecure")
                    .AddJsonBody("This is the body!!")
            );

            processedRequest.Should().NotBeNull();
            processedRequest?.Path.Should().Be("/foo/2");
            processedRequest?.QueryParameters["bar"].Should().Be("123");
            processedRequest?.Body.Should().Be("\"This is the body!!\"");
            processedRequest?.Headers["x-api-key"].Should().Be("1234supersecure");
        }
    }
}