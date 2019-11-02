using System;
using Frank.API.WebDevelopers;
using Frank.ExtensionPoints;
using Frank.Plugins.HttpListener;
using Frank.Plugins.TestHarness;

namespace Frank.Internals
{
    internal class WebApplicationBuilder : IWebApplicationBuilder
    {
        private int _port;
        private Action<IRouteConfigurer> _onRequest;

        public IWebApplicationBuilder OnRequest(Action<IRouteConfigurer> action)
        {
            _onRequest = action;
            return this;
        }

        public IWebApplicationBuilderWithBefore<T> Before<T>(Func<T> onBefore)
        {
            throw new NotImplementedException();
        }

        public IWebApplication StartListeningOn(int port)
        {
            _port = port;
            var app = new WebApplication(
                new HttpListenerServer(_port),
                _onRequest ?? (_ => { })
            );
            app.Start();

            return app;
        }

        public ITestWebApplication StartTesting()
        {
            var testHarnessServer = new TestHarnessServer();
            return new Adapter(
                new WebApplication(
                    testHarnessServer,
                    _onRequest ?? (_ => { })
                ), 
                testHarnessServer
            );
        }
    }
}