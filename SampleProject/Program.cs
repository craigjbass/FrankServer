using System.Threading;
using Frank.API.WebDevelopers.DTO;
using static Frank.API.WebDevelopers.DTO.ResponseBuilders;

namespace SampleProject
{
    class Program
    {
        static void Main()
        {
            Frank.Server
                .Configure()
                .ListenOn(8000)
                .WithRoutes(
                    router =>
                    {
                        router.Get("/test").To(Test);
                        router.Get("/hello").To(Hello);
                    }
                )
                .Build()
                .Start();
            
            SpinWait.SpinUntil(() => false);
        }

        private static Response Test()
        {
            return Ok().WithBody(new {Success = true});
        }

        private static Response Hello(Request request)
        {
            var isHello = request.QueryParameters.ContainsKey("what") && request.QueryParameters["what"] == "hello";

            return Ok().WithBody(isHello ? new {message = "Yes?"} : new {message = "Rude."});
        }
    }
}