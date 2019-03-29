using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Frank.API.WebDevelopers;
using Frank.API.WebDevelopers.DTO;

namespace Frank.Internals
{
    internal class Unroutable : Exception
    {
    }

    internal class RequestRouter : IRouteConfigurer, WebApplication.IRequestRouter
    {
        private readonly Dictionary<string, Func<Response>> _routes;

        public RequestRouter()
        {
            _routes = new Dictionary<string, Func<Response>>();
        }

        public void Get(string path, Func<Response> func)
        {
            _routes.Add(path, func);
        }

        public bool CanRoute(string url)
        {
            return _routes.ContainsKey(url);
        }

        public Response Route(Request request)
        {
            if (_routes.ContainsKey(request.Path))
            {
                return _routes[request.Path]();
            }
            throw new Unroutable();
        }
    }
}