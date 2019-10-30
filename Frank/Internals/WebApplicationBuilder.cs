using System;
using Frank.API.WebDevelopers;
using Frank.ExtensionPoints;
using Frank.Plugins.HttpListener;

namespace Frank.Internals
{
    internal class WebApplicationBuilder : IWebApplicationBuilder
    {
        private RequestRouter _requestRouterConfigurer;
        private IServer _server;
        private int _port;

        public WebApplicationBuilder()
        {
            _server = new HttpListenerServer();
        }

        public IWebApplicationBuilder WithRoutes(Action<IRouteConfigurer> action)
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

        public IWebApplication Build()
        {
            return new WebApplication(_port, _server, _requestRouterConfigurer ?? new RequestRouter());
        }

        public ITestWebApplicationBuilder ForTesting()
        {
            return new TestApplicationBuilder(this);
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
                return new TestHarness(_webApplicationBuilder.Build(), testHarnessServer);
            }
        }
    }
}