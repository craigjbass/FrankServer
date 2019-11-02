using System;
using Frank.API.WebDevelopers;
using Frank.ExtensionPoints;
using Frank.Plugins.HttpListener;
using Frank.Plugins.TestHarness;

namespace Frank.Internals
{
    internal class WebApplicationBuilder : IWebApplicationBuilder
    {
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
            return new BuildWithLifeCycleHooks<T>(this);
        }

        public IWebApplication StartListeningOn(int port)
        {
            var app = CreateApplication(new HttpListenerServer(port));
            app.Start();

            return app;
        }

        public ITestWebApplication StartTesting()
        {
            var testHarnessServer = new TestHarnessServer();
            var webApplication = new Adapter(
                CreateApplication(testHarnessServer), 
                testHarnessServer
            );
            webApplication.Start();
            return webApplication;
        }

        private WebApplication CreateApplication(IServer server)
        {
            return new WebApplication(
                server,
                _onRequest ?? ((_,_2) => { }),
                _onBefore ?? (() => null),
                _onAfter ?? ((_) => {})
            );
        }

        class BuildWithLifeCycleHooks<T> : IWebApplicationBuilderWithBefore<T>
        {
            private readonly WebApplicationBuilder _webApplicationBuilder;

            public BuildWithLifeCycleHooks(WebApplicationBuilder webApplicationBuilder)
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