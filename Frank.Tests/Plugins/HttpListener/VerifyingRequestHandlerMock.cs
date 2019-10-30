using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Frank.API.PluginDevelopers;
using Frank.API.WebDevelopers.DTO;

namespace Frank.Tests.Plugins.HttpListener
{
    internal class VerifyingRequestHandlerMock
    {
        private readonly ConcurrentBag<Request> _requests;
        private Response _respondWith;

        public VerifyingRequestHandlerMock()
        {
            _requests = new ConcurrentBag<Request>();
        }

        public void HandleRequest(Request request, IResponseBuffer buffer)
        {
            _requests.Add(request);
            buffer.SetContentsOfBufferTo(_respondWith);
            buffer.Flush();
        }

        public void SetupRequestHandlerToRespondWith(Response response)
        {
            _respondWith = response;
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

        private Request FirstRequest() => _requests.First();

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
    }
}