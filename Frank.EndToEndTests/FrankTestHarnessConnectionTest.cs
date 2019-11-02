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
            ITestWebApplication webApplication = Server.Configure().StartTesting();

            webApplication.Start();

            var response = webApplication.Execute(new Request());

            response.Status.Should().Be(404);
        }

        [Test]
        public void CanRouteToARoute()
        {
            ITestWebApplication webApplication = Server
                .Configure()
                .OnRequest(
                    c => c.Get("/").To(request => ResponseBuilders.Ok().WithBody(new { a = 123 }))
                ).StartTesting();

            webApplication.Start();

            var response = webApplication.Execute(new Request());

            webApplication.Stop();
            
            response.Status.Should().Be(200);
            Encoding.UTF8.GetString(response.Body).Should().Be("{\"a\":123}");
        }
        
        [Test]
        public void ItCallsOnRequestOncePerRequest()
        {
            var i = 0;
            ITestWebApplication webApplication = Server
                .Configure()
                .OnRequest(
                    _ => i++
                ).StartTesting();

            webApplication.Start();

            webApplication.Execute(new Request());
            webApplication.Execute(new Request());

            webApplication.Stop();
            i.Should().Be(2);
        }
    }
}