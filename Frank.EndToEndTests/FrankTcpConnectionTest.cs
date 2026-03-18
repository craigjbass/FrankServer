using System;
using System.Linq;
using System.Net;
using System.Text;
using FluentAssertions;
using Frank.API.WebDevelopers;
using Frank.API.WebDevelopers.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using static Frank.API.WebDevelopers.DTO.ResponseConstructors;
// ReSharper disable PossibleNullReferenceException

namespace Frank.EndToEndTests
{
    [NonParallelizable]
    public class FrankTcpConnectionTest
    {
        private IWebApplicationBuilder _builder;
        private IWebApplication _webApplication;
        private RestResponse _response;
        private int _port;
        private bool _expectExceptionInTeardown;

        private void MakeGetRequest(string path)
        {
            var request = new RestRequest(path, Method.Get);
            var client = new RestClient($"http://127.0.0.1:{_port}/");

            _response = client.Execute(request);
        }

        private RestResponse TheResponse()
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
            _port = 8019;
            _builder = Server.Configure();
            _expectExceptionInTeardown = false;
        }

        [TearDown]
        public void TearDown()
        {
            var thrown = false;
            try
            {
                StopFrank();
            }
            catch (Exception)
            {
                if (!_expectExceptionInTeardown) throw;

                thrown = true;
            }

            if (_expectExceptionInTeardown && !thrown)
            {
                throw new Exception("Expected an exception, but one was not thrown.");
            }
        }

        private void StartFrank()
        {
            _webApplication = _builder.StartListeningOn(_port);
        }

        private void StartFrankWithRoutes(Action<IRouteConfigurer> action)
        {
            _builder.OnRequest(action);
            StartFrank();
        }

        private void StopFrank()
        {
            _webApplication.Stop();
        }

        [Test]
        public void CanServeNotFoundTwice()
        {
            StartFrank();

            MakeGetRequest("/");
            ((int)TheResponse().StatusCode).Should().Be(404);

            MakeGetRequest("/another-route");
            ((int)TheResponse().StatusCode).Should().Be(404);
        }

        [TestCase("/custom", 200, "Custom Body", "X-Header-Here", "A Value")]
        [TestCase("/error", 500, "An error occurred", "X-Error", "Yes")]
        public void ResponseContainsARawBodyStatusAndHeaders(
            string routePath, int status, string body, string headerKey, string headerValue
        )
        {
            _builder
                .OnRequest(route =>
                {
                    route.Get(routePath).To(() =>
                    {
                        return NewResponseWithStatus(status)
                            .BodyFromString(body)
                            .WithHeader(headerKey, headerValue);
                    });
                });

            StartFrank();

            var response = new RestClient("http://127.0.0.1:8019/").Execute(
                new RestRequest(routePath, Method.Get)
            );

            response.Content.Should().Be(body);
            ((int)response.StatusCode).Should().Be(status);
            response.Headers.Concat(response.ContentHeaders).FirstOrDefault(h => h.Name == headerKey).Value.Should().Be(headerValue);
        }

        [TestCase("/success")]
        [TestCase("/okay")]
        public void CanServeOk(string route)
        {
            StartFrankWithRoutes(router => { router.Get(route).To(Ok); });
            MakeGetRequest(route);
            ((int)TheResponse().StatusCode).Should().Be(200);
        }

        [Test]
        public void CanServeACreatedStatusCode()
        {
            _port = 8090;
            StartFrankWithRoutes(router => { router.Get("/created").To(Created); });
            MakeGetRequest("/created");
            ((int)TheResponse().StatusCode).Should().Be(201);
        }

        [Test]
        public void CanSerializeJsonResponseIntoHttpBody()
        {
            StartFrankWithRoutes(
                router =>
                {
                    router.Get("/foo/2")
                        .To(() => Ok().WithJsonBody(new {Id = 2}));
                }
            );

            MakeGetRequest("/foo/2");

            ((int)TheResponse().StatusCode).Should().Be(200);
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
                new RestRequest("/foo/2", Method.Get)
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
                new RestRequest("/foo/2", Method.Post)
                    .AddQueryParameter("bar", "123")
                    .AddHeader("X-Api-Key", "1234supersecure")
                    .AddStringBody(JsonConvert.SerializeObject("This is the body!!"), ContentType.Json)
            );

            processedRequest.Should().NotBeNull();
            processedRequest?.Path.Should().Be("/foo/2");
            processedRequest?.QueryParameters["bar"].Should().Be("123");
            processedRequest?.Body.Should().Be("\"This is the body!!\"");
            processedRequest?.Headers["x-api-key"].Should().Be("1234supersecure");
        }

        [Test]
        public void CanExecuteBeforeAndAfterHandlers()
        {
            var customContext = new LifecycleHooksSpy();
            _builder
                .Before(() =>
                {
                    customContext.Before();
                    return customContext;
                })
                .After(context => { context.After(); })
                .OnRequest(((route, context) =>
                {
                    context.Request();
                    route.Get("/").To(() =>
                    {
                        context.RouteHandler();
                        throw new Exception();
                    });
                }));

            StartFrank();

            new RestClient("http://127.0.0.1:8019/").Execute(
                new RestRequest("/", Method.Get)
            );

            customContext.OrderThatMethodsWereCalled.Should().ContainInOrder(
                "before", "request", "route-handler", "after"
            );

            _expectExceptionInTeardown = true;
        }
    }
}