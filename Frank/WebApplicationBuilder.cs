using System;

namespace Frank
{
    public class WebApplicationBuilder
    {
        private Router _routerConfigurer;

        public void WithRoutes(Action<IRouteConfigurer> action)
        {
            _routerConfigurer = new Router();
            action(_routerConfigurer);
        }

        public void ListenOn(string ip, string port)
        {
        }

        public IWebApplication Build()
        {
            return new WebApplication(new HttpListenerServer(), _routerConfigurer ?? new Router());
        }
    }
}