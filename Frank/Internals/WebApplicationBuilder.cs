using System;
using Frank.API.WebDevelopers;
using Frank.Plugins.HttpListener;

namespace Frank.Internals
{
    internal class WebApplicationBuilder : IWebApplicationBuilder
    {
        private RequestRouter _requestRouterConfigurer;

        public IWebApplicationBuilder WithRoutes(Action<IRouteConfigurer> action)
        {
            _requestRouterConfigurer = new RequestRouter();
            action(_requestRouterConfigurer);
            return this;
        }

        public IWebApplicationBuilder ListenOn(string ip, string port)
        {
            return this;
        }

        public IWebApplication Build()
        {
            return new WebApplication(new HttpListenerServer(), _requestRouterConfigurer ?? new RequestRouter());
        }
    }
}