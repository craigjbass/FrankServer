using System;
using Frank.API.WebDevelopers;
using Frank.API.WebDevelopers.DTO;
using static Frank.API.WebDevelopers.DTO.ResponseBuilders;


namespace SampleProject
{
    public class SampleProjectApp
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
        public IWebApplicationBuilder CreateApplication()
        {
            return Frank.Server
                .Configure()
                .Before(() => { return new ExampleDatabaseConnection(); })
                .After(dbConnection => { dbConnection.Close(); })
                .OnRequest(
                    (route, dbConnection) =>
                    {
                        route.Get("/test").To(Test);
                        route.Get("/hello").To(Hello);
                    }
                );
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