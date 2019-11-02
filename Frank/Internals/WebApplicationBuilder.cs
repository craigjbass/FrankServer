using System;
using Frank.API.WebDevelopers;
using Frank.Plugins.HttpListener;
using Frank.Plugins.TestHarness;

namespace Frank.Internals
{
    internal class WebApplicationBuilder : IWebApplicationBuilder
    {
        private int _port;
        private Action<IRouteConfigurer, object> _onRequest;
        private Func<object> _onBefore;
        private Action<object> _onAfter;
        
        public IWebApplicationBuilder OnRequest(Action<IRouteConfigurer> action)
        {
            _onRequest = ((r,_) => action(r));
            return this;
        }
        
        public IWebApplicationBuilderWithBefore<T> Before<T>(Func<T> onBefore)
        {
            _onBefore = () => onBefore();
            return new Foo<T>(this);
        }

        public IWebApplication StartListeningOn(int port)
        {
            _port = port;
            var app = new WebApplication(
                new HttpListenerServer(_port),
                _onRequest ?? ((_,_2) => { }),
                _onBefore ?? (() => null),
                _onAfter ?? ((_) => {})
            );
            app.Start();

            return app;
        }

        public ITestWebApplication StartTesting()
        {
            var testHarnessServer = new TestHarnessServer();
            var iTestWebApplication = new Adapter(
                new WebApplication(
                    testHarnessServer,
                    _onRequest ?? ((_,_2) => { }),
                    _onBefore ?? (() => null),
                    _onAfter ?? ((_) => {})
                ), 
                testHarnessServer
            );
            iTestWebApplication.Start();
            return iTestWebApplication;
        }

        class Foo<T> : IWebApplicationBuilderWithBefore<T>
        {
            private readonly WebApplicationBuilder _webApplicationBuilder;

            public Foo(WebApplicationBuilder webApplicationBuilder)
            {
                _webApplicationBuilder = webApplicationBuilder;
            }

            public IWebApplicationBuilderWithBefore<T> After(Action<T> onAfter)
            {
                _webApplicationBuilder._onAfter = (context) => onAfter((T) context); 
                return this;
            }

            public IWebApplicationBuilder OnRequest(Action<IRouteConfigurer, T> action)
            {
                _webApplicationBuilder._onRequest = (a, context) => action(a, (T)context);
                return _webApplicationBuilder;
            }
        }
    }
}