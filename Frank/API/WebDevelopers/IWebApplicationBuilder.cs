using System;

namespace Frank.API.WebDevelopers
{
    public interface IWebApplicationBuilder
    {
        IWebApplicationBuilder OnRequest(Action<IRouteConfigurer> action);
        IWebApplicationBuilderWithBefore<T> Before<T>(Func<T> onBefore);
        IWebApplication StartListeningOn(int port);
        ITestWebApplication StartTesting();
    }
    
    public interface IWebApplicationBuilderWithBefore<T> : IWebApplicationBuilder
    {
        IWebApplicationBuilderWithBefore<T> After(Action<T> onAfter);
        IWebApplicationBuilderWithBefore<T> OnRequest(Action<IRouteConfigurer, T> action);
    }
}