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
        private VerifyingRequestHandlerMock _requestHandlerMock;
        private HttpListenerServer _httpListenerServer;

        private void StartServer()
        {
            _httpListenerServer.Start();
            _httpListenerServer.RegisterRequestHandler(_requestHandlerMock.HandleRequest);
        }

        [SetUp]
        public void SetUp()
        {
            _httpListenerServer = new HttpListenerServer(8020);
            _requestHandlerMock = new VerifyingRequestHandlerMock();
        }

        [TearDown]
        public void TearDown() => _httpListenerServer.Stop();

        private void MakeRequest(string url, string path, Method method, Action<RestRequest> requestBuilder = null)
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
        public void TimesOutWhenNoRequestProcessorRegistered()
        {
            StartServer();
            MakeRequest("http://127.0.0.1:8020", "/", Method.GET);
            AssertTimedOut();
        }

        [Test]
        public void CanServeRequestFromRequestProcessor()
        {
            StartServer();
            _requestHandlerMock.SetupRequestHandlerToRespondWith(Ok().BodyFromString("Hello world"));

            MakeRequest("http://127.0.0.1:8020", "/", Method.GET);

            AssertContentIs("Hello world");
            AssertStatusIsOk();
        }


        [Test]
        public void CanDeserialiseEmptyHttpRequest()
        {
            StartServer();
            _requestHandlerMock.SetupRequestHandlerToRespondWith(Ok().BodyFromString("Hello world"));

            MakeAMinimalGetRequest("http://127.0.0.1:8020");

            _requestHandlerMock.AssertPathIs("/");
            _requestHandlerMock.AssertThatThereAreNoQueryParameters();
            _requestHandlerMock.AssertThatTheRequestBodyIsEmpty();
            _requestHandlerMock.AssertHeaderCountIs(2);
            _requestHandlerMock.AssertHeadersCaseInsensitivelyContain("Connection", "Keep-Alive");
            _requestHandlerMock.AssertHeadersCaseInsensitivelyContain("Host", "127.0.0.1:8020");
        }

        [Test]
        public void CanDeserialiseHttpRequest()
        {
            StartServer();
            _requestHandlerMock.SetupRequestHandlerToRespondWith(Ok().BodyFromString("Hello world"));

            MakeRequest(
                "http://127.0.0.1:8020",
                "/asd", Method.POST,
                r =>
                {
                    r.AddQueryParameter("z", "123");
                    r.AddHeader("Content-Type", "application/json");
                    r.AddJsonBody(new { });
                }
            );

            _requestHandlerMock.AssertPathIs("/asd");

            _requestHandlerMock.AssertQueryParametersContain("z", "123");
            _requestHandlerMock.AssertThatTheRequestBodyIs("{}");
            _requestHandlerMock.AssertHeadersCaseInsensitivelyContain("Content-Type", "application/json");
        }


        [Test]
        public void CanDeserialiseHttpRequest2()
        {
            StartServer();
            _requestHandlerMock.SetupRequestHandlerToRespondWith(Ok().BodyFromString("Hello world"));

            MakeRequest(
                "http://127.0.0.1:8020",
                "/asd", Method.GET,
                r => { r.AddParameter("j", "nice"); }
            );

            _requestHandlerMock.AssertPathIs("/asd");
            _requestHandlerMock.AssertQueryParametersContain("j", "nice");
            _requestHandlerMock.AssertThatTheRequestBodyIsEmpty();
        }
    }
}