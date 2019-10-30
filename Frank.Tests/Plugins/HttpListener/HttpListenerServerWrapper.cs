using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using Frank.API.WebDevelopers.DTO;
using Frank.Plugins.HttpListener;

namespace Frank.Tests.Plugins.HttpListener
{
    internal class HttpListenerServerWrapper : IVerifyingRequestHandler, IHttpServerWrapper
    {
        private ConcurrentBag<Request> _requests;
        private HttpListenerServer _listener;

        public HttpListenerServerWrapper(HttpListenerServer httpListenerServer)
        {
            _requests = new ConcurrentBag<Request>();
            _listener = httpListenerServer;
        }

        public void AssertHeadersCaseInsensitivelyContain(string expectedKey, string expectedValue)
        {
            FirstRequest().Headers.Should()
                .Contain(new KeyValuePair<string, string>(
                    expectedKey.ToLower(),
                    expectedValue));
            FirstRequest().Headers.Should()
                .Contain(new KeyValuePair<string, string>(
                    expectedKey.ToUpper(),
                    expectedValue));

            FirstRequest().Headers.Should()
                .Contain(new KeyValuePair<string, string>(
                    CapitalizeFirstLetter(expectedKey.ToLower()),
                    expectedValue));
        }

        private Request FirstRequest()
        {
            WaitForRequest();
            return _requests.First();
        }

        private static string CapitalizeFirstLetter(string expectedKey)
        {
            return expectedKey.First().ToString().ToUpper() + String.Join("", expectedKey.Skip(1));
        }

        public void AssertHeaderCountIs(int expectedNumberOfHeaders) =>
            FirstRequest().Headers.Should().HaveCount(expectedNumberOfHeaders);

        public void AssertThatTheRequestBodyIsEmpty() => FirstRequest().Body.Should().Be("");

        public void AssertThatThereAreNoQueryParameters() =>
            FirstRequest().QueryParameters.Should().BeEmpty();

        public void AssertPathIs(string expectedPath) => FirstRequest().Path.Should().Be(expectedPath);

        public void AssertQueryParametersContain(string expectedKey, string expectedValue) =>
            FirstRequest().QueryParameters[expectedKey].Should().Be(expectedValue);

        public void AssertThatTheRequestBodyIs(string expectedBody)
        {
            FirstRequest().Body.Should().Be(expectedBody);
        }

        public void SetupRequestHandlerToRespondWith(Response response)
        {
            Task.Run(() =>
            {
                _listener.RegisterRequestHandler((request, buffer) =>
                {
                    _requests.Add(request);
                    buffer.SetContentsOfBufferTo(response);
                    buffer.Flush();
                });
            });
        }

        public void Start()
        {
            _listener.Start(8020);
        }

        public string Host => "http://127.0.0.1:8020";
        public void WaitForRequest() => SpinWait.SpinUntil(() => !_requests.IsEmpty, (int) 30.Seconds().TotalMilliseconds);

        public void Stop()
        {
            _listener.Stop();
        }
    }
}