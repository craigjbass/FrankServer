﻿using System;
using Frank.API.WebDevelopers;
using Frank.ExtensionPoints;
using Frank.Plugins.HttpListener;
using Frank.Plugins.TestHarness;

namespace Frank.Internals
{
    internal class WebApplicationBuilder : IWebApplicationBuilder
    {
        private RequestRouter _requestRouterConfigurer;
        private IServer _server;
        private int _port;

        public IWebApplicationBuilder OnRequest(Action<IRouteConfigurer> action)
        {
            _requestRouterConfigurer = new RequestRouter();
            action(_requestRouterConfigurer);
            return this;
        }

        public IWebApplicationBuilder ListenOn(int port)
        {
            _port = port;
            return this;
        }

        public ITestWebApplicationBuilder ForTesting()
        {
            return new TestApplicationBuilder(this);
        }

        public IWebApplicationBuilderWithBefore<T> Before<T>(Func<T> onBefore)
        {
            throw new NotImplementedException();
        }

        public IWebApplication Build()
        {
            return new WebApplication(_server ?? new HttpListenerServer(_port),
                _requestRouterConfigurer ?? new RequestRouter()
            );
        }

        private class TestApplicationBuilder : ITestWebApplicationBuilder
        {
            private readonly WebApplicationBuilder _webApplicationBuilder;

            public TestApplicationBuilder(WebApplicationBuilder webApplicationBuilder)
            {
                _webApplicationBuilder = webApplicationBuilder;
            }

            public ITestWebApplication Build()
            {
                var testHarnessServer = new TestHarnessServer();
                _webApplicationBuilder._server = testHarnessServer;
                return new Adapter(_webApplicationBuilder.Build(), testHarnessServer);
            }
        }
    }
}