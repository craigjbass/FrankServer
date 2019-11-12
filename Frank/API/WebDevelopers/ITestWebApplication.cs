using System.Runtime.CompilerServices;
using Frank.API.WebDevelopers.DTO;

namespace Frank.API.WebDevelopers
{
    public interface ITestWebApplication : IWebApplication
    {
        ITestResponse Execute(Request request);
    }

    public interface ITestResponse
    {
        int Status { get; }
        string Body { get; }
        dynamic DeserializeBody();
    }
}