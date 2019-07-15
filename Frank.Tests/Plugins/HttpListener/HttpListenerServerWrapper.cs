using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Frank.API.WebDevelopers.DTO;
using Frank.ExtensionPoints;
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
            _requests.First().Headers.Should()
                .Contain(new KeyValuePair<string, string>(
                    expectedKey.ToLower(),
                    expectedValue));
            _requests.First().Headers.Should()
                .Contain(new KeyValuePair<string, string>(
                    expectedKey.ToUpper(),
                    expectedValue));

            _requests.First().Headers.Should()
                .Contain(new KeyValuePair<string, string>(
                    CapitalizeFirstLetter(expectedKey.ToLower()),
                    expectedValue));
        }

        private static string CapitalizeFirstLetter(string expectedKey)
        {
            return expectedKey.First().ToString().ToUpper() + String.Join("", expectedKey.Skip(1));
        }

        public void AssertHeaderCountIs(int expectedNumberOfHeaders) =>
            _requests.First().Headers.Should().HaveCount(expectedNumberOfHeaders);

        public void AssertThatTheRequestBodyIsEmpty() => _requests.First().Body.Should().Be("");

        public void AssertThatThereAreNoQueryParameters() =>
            _requests.First().QueryParameters.Should().BeEmpty();

        public void AssertPathIs(string expectedPath) => _requests.First().Path.Should().Be(expectedPath);

        public void AssertQueryParametersContain(string expectedKey, string expectedValue) =>
            _requests.First().QueryParameters[expectedKey].Should().Be(expectedValue);

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
            _listener.Start(new[] {new ListenOn {HostName = "127.0.0.1", Port = 8020}});
        }

        public string Host => "http://127.0.0.1:8020";

        public void Stop()
        {
            _listener.Stop();
        }
    }
}