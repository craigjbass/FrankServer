using System;
using System.Net;
using FluentAssertions;
using Frank.API.WebDevelopers.DTO;
using Frank.Plugins.HttpListener;
using NUnit.Framework;
using RestSharp;
using static Frank.API.WebDevelopers.DTO.ResponseBuilders;

namespace Frank.Tests.Plugins.HttpListener
{
    [SingleThreaded]
    public class HttpListenerServerTests
    {
        private IRestResponse _response;
        private IHttpServerWrapper _server;
        private IVerifyingRequestHandler _requestHandler;

        [SetUp]
        public void SetUp()
        {
            var httpListenerServerWrapper = new HttpListenerServerWrapper(new HttpListenerServer());
            _requestHandler = httpListenerServerWrapper;
            _server = httpListenerServerWrapper;
        }

        [TearDown]
        public void TearDown() => _server.Stop();

        private void MakeGetRequest(string url, string path, Method method, Action<RestRequest> requestBuilder = null)
        {
            var request = new RestRequest(path, method);

            (requestBuilder ?? (r => { }))(request);

            request.Timeout = 100;
            var client = new RestClient(url);
            _response = client.Execute(request);
        }

        private static void MakeAMinimalGetRequest(string url)
        {
            var request = WebRequest.Create(url);
            request.Method = "GET";
            request.GetResponse();
        }

        private void AssertStatusCodeIs(int expected) => _response.StatusCode.Should().Be(expected);

        private void AssertTimedOut() => _response.ResponseStatus.Should().Be(ResponseStatus.TimedOut);

        private void AssertStatusIsOk() => AssertStatusCodeIs(200);

        private void AssertContentIs(string expectedContent) => _response.Content.Should().Be(expectedContent);

        [Test]
        public void RespondsWith404OnAnyUnregisteredHostname()
        {
            _server.Start();
            MakeGetRequest("http://localhost:8020/", "/", Method.GET);
            AssertStatusCodeIs(404);
        }

        [Test]
        public void TimesOutWhenNoRequestProcessorRegistered()
        {
            _server.Start();
            MakeGetRequest(_server.Host, "/", Method.GET);
            AssertTimedOut();
        }

        [Test]
        public void CanServeRequestFromRequestProcessor()
        {
            _server.Start();
            _requestHandler.SetupRequestHandlerToRespondWith(Ok().BodyFromString("Hello world"));

            MakeGetRequest(_server.Host, "/", Method.GET);

            AssertContentIs("Hello world");
            AssertStatusIsOk();
        }


        [Test]
        public void CanDeserialiseEmptyHttpRequest()
        {
            _server.Start();
            _requestHandler.SetupRequestHandlerToRespondWith(Ok().BodyFromString("Hello world"));

            MakeAMinimalGetRequest(_server.Host);

            _requestHandler.AssertPathIs("/");
            _requestHandler.AssertThatThereAreNoQueryParameters();
            _requestHandler.AssertThatTheRequestBodyIsEmpty();
            _requestHandler.AssertHeaderCountIs(2);
            _requestHandler.AssertHeadersCaseInsensitivelyContain("Connection", "Keep-Alive");
            _requestHandler.AssertHeadersCaseInsensitivelyContain("Host", "127.0.0.1:8020");
        }

        [Test]
        public void CanDeserialiseHttpRequest()
        {
            _server.Start();
            _requestHandler.SetupRequestHandlerToRespondWith(Ok().BodyFromString("Hello world"));

            MakeGetRequest(
                _server.Host,
                "/asd", Method.POST,
                r =>
                {
                    r.AddQueryParameter("z", "123");
                    r.AddHeader("Content-Type", "application/json");
                    r.AddJsonBody(new { });
                }
            );

            _requestHandler.AssertPathIs("/asd");

            _requestHandler.AssertQueryParametersContain("z", "123");
            _requestHandler.AssertThatTheRequestBodyIs("{}");
            _requestHandler.AssertHeadersCaseInsensitivelyContain("Content-Type", "application/json");
        }


        [Test]
        public void CanDeserialiseHttpRequest2()
        {
            _server.Start();
            _requestHandler.SetupRequestHandlerToRespondWith(Ok().BodyFromString("Hello world"));

            MakeGetRequest(
                _server.Host,
                "/asd", Method.GET,
                r => { r.AddParameter("j", "nice"); }
            );

            _requestHandler.AssertPathIs("/asd");
            _requestHandler.AssertQueryParametersContain("j", "nice");
            _requestHandler.AssertThatTheRequestBodyIsEmpty();
        }
    }
}