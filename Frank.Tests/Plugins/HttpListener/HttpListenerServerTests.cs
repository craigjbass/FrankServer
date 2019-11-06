using System;
using System.Linq;
using System.Net;
using FluentAssertions;
using Frank.API.WebDevelopers.DTO;
using Frank.Plugins.HttpListener;
using NUnit.Framework;
using RestSharp;
using static Frank.API.WebDevelopers.DTO.ResponseConstructors;
// ReSharper disable PossibleNullReferenceException

namespace Frank.Tests.Plugins.HttpListener
{
    [NonParallelizable]
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

        private static void MakeAnEmptyGetRequest(string url)
        {
            var request = WebRequest.Create(url);
            request.Method = "GET";
            request.GetResponse();
        }
        
        private void AssertThatThisHeaderIsPresent(string key, string expected) 
            => _response.Headers
                .FirstOrDefault(p => p.Name == key)
                .Value
                .Should()
                .Be(expected);

        private void AssertStatusCodeIs(int expected) => _response.StatusCode.Should().Be(expected);

        private void AssertTimedOut() => _response.ResponseStatus.Should().Be(ResponseStatus.TimedOut);

        private void AssertStatusIsOk() => AssertStatusCodeIs(200);

        private void AssertContentIs(string expectedContent) => _response.Content.Should().Be(expectedContent);

        [Test]
        public void TimesOutWhenNoRequestProcessorRegistered()
        {
            _httpListenerServer.Start();
            MakeRequest("http://127.0.0.1:8020", "/", Method.GET);
            AssertTimedOut();
        }

        [Test]
        public void CanDeserialiseAnEmptyHttpGetRequest()
        {
            StartServer();
            _requestHandlerMock.SetupRequestHandlerToRespondWith(Ok().BodyFromString("Hello world"));

            MakeAnEmptyGetRequest("http://127.0.0.1:8020");

            _requestHandlerMock.AssertPathIs("/");
            _requestHandlerMock.AssertThatThereAreNoQueryParameters();
            _requestHandlerMock.AssertThatTheRequestBodyIsEmpty();
            _requestHandlerMock.AssertHeaderCountIs(2);
            _requestHandlerMock.AssertHeadersCaseInsensitivelyContain("Connection", "Keep-Alive");
            _requestHandlerMock.AssertHeadersCaseInsensitivelyContain("Host", "127.0.0.1:8020");
        }

        [Test]
        public void CanDeserialiseAnHttpGetRequest()
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

        [Test]
        public void CanDeserialiseAnHttpPostRequest()
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
        public void CanSerialiseAnHttpResponse()
        {
            StartServer();
            _requestHandlerMock.SetupRequestHandlerToRespondWith(
                Ok()
                    .WithHeader("X-Authentication-Scheme", "DangerousBespoke")
                    .WithHeader("Content-Type", "plain/text")
                    .BodyFromString("Hello world")
            );

            MakeRequest("http://127.0.0.1:8020", "/", Method.GET);

            AssertThatThisHeaderIsPresent("X-Authentication-Scheme", "DangerousBespoke");
            AssertThatThisHeaderIsPresent("Content-Type", "plain/text");
            var serverHeader = (string) _response.Headers.FirstOrDefault(h => h.Name == "Server").Value;
            serverHeader.Should().StartWith("FrankServer");
            AssertContentIs("Hello world");
            AssertStatusIsOk();
        }
    }
}