using Frank.API.WebDevelopers;
using Frank.API.WebDevelopers.DTO;
using static Frank.API.WebDevelopers.DTO.ResponseConstructors;


namespace SampleProject
{
    public class SampleProjectApp
    {
        class ExampleDatabaseConnection
        {
            public void Close()
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
                        route.Get("/test").To(Test)
                            .Get("/hello").To(Hello)
                            .Get("/three").To(Test);
                    }
                );
        }
        
        
        private static Response Test()
        {
            return Ok().WithJsonBody(new {Success = true});
        }

        private static Response Hello(Request request)
        {
            var isHello = request.QueryParameters.ContainsKey("what") && request.QueryParameters["what"] == "hello";

            return Ok().WithJsonBody(isHello ? new {message = "Yes?"} : new {message = "Rude."});
        }
    }
}