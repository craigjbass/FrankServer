using System;
using Frank.Internals;

namespace Frank.API.WebDevelopers
{
    public interface IWebApplicationBuilder
    {
        IWebApplicationBuilder WithRoutes(Action<IRouteConfigurer> action);
        IWebApplicationBuilder ListenOn(string ip, string port);
        IWebApplication Build();
        ITestWebApplicationBuilder ForTesting();
    }
}