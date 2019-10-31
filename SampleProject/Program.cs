using System.Threading;
using Frank.API.WebDevelopers.DTO;
using static Frank.API.WebDevelopers.DTO.ResponseBuilders;

namespace SampleProject
{
    class Program
    {
        class ExampleDatabaseConnection
        {
            public void Close()
            {
            }

            public void Query()
            {
            }
        }
        static void Main()
        {
            Frank.Server
                .Configure()
                .ListenOn(8000)
                .Before(() => { return new ExampleDatabaseConnection(); })
                .After(dbConnection =>
                {
                    dbConnection.Close();
                })
                .OnRequest(
                    (route, dbConnection) =>
                    {
                        route.Get("/test").To(Test);
                        route.Get("/hello").To(Hello);
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