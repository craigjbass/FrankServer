using System.Runtime.CompilerServices;
using Frank.API.WebDevelopers;
using Frank.Internals;

[assembly: InternalsVisibleTo("Frank.Tests")]
// ReSharper disable once CheckNamespace
namespace Frank
{
    public static class Server
    {
        public static IWebApplicationBuilder Configure()
        {
            return new WebApplicationBuilder();
        }
    }
}