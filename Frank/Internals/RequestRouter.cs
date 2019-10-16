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

    internal class RequestRouter :
        IRouteConfigurer,
        IRouteToConfigurer,
        WebApplication.IRequestRouter
    {
        private readonly Dictionary<string, Func<Request, Response>> _routes;
        private string _path;

        public RequestRouter()
        {
            _routes = new Dictionary<string, Func<Request, Response>>();
        }

        public IRouteToConfigurer Get(string path)
        {
            _path = path;
            return this;
        }

        public IRouteToConfigurer Post(string path)
        {
            _path = path;
            return this;
        }

        public IRouteConfigurer To(Func<Response> func)
        {
            _routes.Add(_path, _ => func());
            return this;
        }

        public IRouteConfigurer To(Func<Request, Response> func)
        {
            _routes.Add(_path, func);
            return this;
        }

        public bool CanRoute(string url) => _routes.ContainsKey(url);

        public Response Route(Request request)
        {
            if (IsUnroutable(request))
                throw new Unroutable();

            return _routes[request.Path](request);
        }

        private bool IsUnroutable(Request request) => 
            request.Path == null || !CanRoute(request.Path);
    }
}