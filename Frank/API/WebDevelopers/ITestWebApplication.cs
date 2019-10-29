using Frank.API.WebDevelopers.DTO;

namespace Frank.API.WebDevelopers
{
    public interface ITestWebApplication : IWebApplication
    {
        Response Execute(Request request);
    }
}