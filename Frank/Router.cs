using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Frank.DTO;

[assembly: InternalsVisibleTo("Frank.Tests")]
namespace Frank
{
    internal class Router : IRouteConfigurer, WebApplication.IRouter
    {
        private readonly Dictionary<string, Func<int>> _routes = new Dictionary<string, Func<int>>();

        public void Get(string path, Func<int> func)
        {
            _routes.Add(path, func);
        }

        public bool CanRoute(string url)
        {
            return _routes.ContainsKey(url);
        }

        public Response Route(Request request)
        {
            return new Response
            {
                Status = _routes[request.Path]()
            };
        }
    }
}