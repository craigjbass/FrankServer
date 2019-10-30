using System.Text;
using FluentAssertions;
using Frank.API.WebDevelopers;
using Frank.API.WebDevelopers.DTO;
using NUnit.Framework;

namespace Frank.EndToEndTests
{
    public class FrankTestHarnessConnectionTest
    {
        [Test]
        public void CanMakeTestRequestAndRespondWith404()
        {
            ITestWebApplication webApplication = Server.Configure().ForTesting().Build();

            webApplication.Start();

            var response = webApplication.Execute(new Request());

            response.Status.Should().Be(404);
        }

        [Test]
        public void CanRouteToARoute()
        {
            ITestWebApplication webApplication = Server
                .Configure()
                .WithRoutes(
                    c => c.Get("/").To(request => ResponseBuilders.Ok().WithBody(new { a = 123 }))
                ).ForTesting()
                .Build();

            webApplication.Start();

            var response = webApplication.Execute(new Request());

            webApplication.Stop();
            
            response.Status.Should().Be(200);
            Encoding.UTF8.GetString(response.Body).Should().Be("{\"a\":123}");
        }
        
        [Test]
        public void CanDoThing()
        {
            ITestWebApplication webApplication = Server
                .Configure()
                .WithRoutes(
                    c => c.Get("/").To(request => ResponseBuilders.Ok().WithBody(new { a = 123 }))
                ).ForTesting()
                .Build();

            webApplication.Start();

            var response = webApplication.Execute(new Request());

            webApplication.Stop();
            
            response.Status.Should().Be(200);
            Encoding.UTF8.GetString(response.Body).Should().Be("{\"a\":123}");
        }
    }
}