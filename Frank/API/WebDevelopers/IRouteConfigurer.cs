using System;
using Frank.API.WebDevelopers.DTO;

namespace Frank.API.WebDevelopers
{
    public interface IRouteConfigurer
    {
        void Get(string path, Func<Response> func);
        void Get(string path, Func<Request, Response> func);
    }
}