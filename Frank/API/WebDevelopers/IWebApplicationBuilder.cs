using System;

namespace Frank.API.WebDevelopers
{
    public interface IWebApplicationBuilder
    {
        IWebApplicationBuilder OnRequest(Action<IRouteConfigurer> action);
        IWebApplicationBuilder ListenOn(int port);
        IWebApplication Build();
        ITestWebApplicationBuilder ForTesting();
        IWebApplicationBuilderWithBefore<T> Before<T>(Func<T> onBefore);
    }
    
    public interface IWebApplicationBuilderWithBefore<T> : IWebApplicationBuilder
    {
        IWebApplicationBuilderWithBefore<T> After(Action<T> onAfter);
        IWebApplicationBuilderWithBefore<T> OnRequest(Action<IRouteConfigurer, T> action);
    }
}