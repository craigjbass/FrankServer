using System.Runtime.CompilerServices;
using Frank.API.WebDevelopers;
using Frank.Internals;

[assembly: InternalsVisibleTo("Frank.Tests")]
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