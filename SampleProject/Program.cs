using System.Threading;
using Frank.API.WebDevelopers.DTO;
using static Frank.API.WebDevelopers.DTO.ResponseBuilders;

namespace SampleProject
{
    class Program
    {
        static void Main()
        {
            System.Frank
                .Configure()
                .ListenOn("127.0.0.1", "8000")
                .WithRoutes(
                    router =>
                    {
                        router.Get("/test", () => Ok().WithBody(new {Success = true}));
                        router.Get(
                            "/hello",
                            request => Ok()
                                .WithBody(request.QueryParameters.ContainsKey("what") &&
                                          request.QueryParameters["what"] == "hello"
                                    ? new { message = "Yes?" }
                                    : new { message = "Rude." }
                                )
                        );
                    }
                )
                .Build()
                .Start();


            SpinWait.SpinUntil(() => false);
        }
    }
}