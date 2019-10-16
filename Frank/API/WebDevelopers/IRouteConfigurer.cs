using System;
using Frank.API.WebDevelopers.DTO;

namespace Frank.API.WebDevelopers
{
    public interface IRouteConfigurer
    {
        IRouteToConfigurer Get(string path);
        IRouteToConfigurer Post(string path);
    }

    public interface IRouteToConfigurer
    {
        IRouteConfigurer To(Func<Response> func);
        IRouteConfigurer To(Func<Request, Response> func);
    }
}