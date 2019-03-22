using Frank;
using Frank.API.WebDevelopers;
using Frank.Internals;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class Frank
    {
        public static IWebApplicationBuilder Configure()
        {
            return new WebApplicationBuilder();
        }
    }
}