using System;
using Frank.Internals;

namespace Frank.API.WebDevelopers
{
    public interface IWebApplicationBuilder
    {
        IWebApplicationBuilder WithRoutes(Action<IRouteConfigurer> action);
        IWebApplicationBuilder ListenOn(int port);
        IWebApplication Build();
        ITestWebApplicationBuilder ForTesting();
    }
}