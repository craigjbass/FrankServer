using System;
using System.Net;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using RestSharp;

namespace Frank.Test
{
    public class FrankTest
    {
        private WebApplicationBuilder _builder;
        private IWebApplication _webApplication;
        private IRestResponse _response;

        [SetUp]
        public void SetUp()
        {
            _builder = new WebApplicationBuilder();
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

        private void StopFrank()
        {
            _webApplication.Stop();
        }

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

        [Test]
        public void CanServeOk()
        {
            _builder.WithRoutes(router => { router.Get("/success", () => 200); });
            StartFrank();

            MakeGetRequest("/success");
            TheResponse().StatusCode.Should().Be(200);
        }

        [Test]
        public void CanServeOk2()
        {
            _builder.WithRoutes(router => { router.Get("/okay", () => 200); });
            StartFrank();

            MakeGetRequest("/okay");
            TheResponse().StatusCode.Should().Be(200);
        }
        
        
        [Test]
        public void CanServeANoContentStatusCode()
        {
            _builder.WithRoutes(router => { router.Get("/no-content", () => 201); });
            StartFrank();

            MakeGetRequest("/no-content");
            TheResponse().StatusCode.Should().Be(201);
        }
    }
}