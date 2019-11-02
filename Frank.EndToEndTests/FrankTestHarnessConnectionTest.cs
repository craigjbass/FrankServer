using System;
using System.Text;
using FluentAssertions;
using Frank.API.WebDevelopers;
using Frank.API.WebDevelopers.DTO;
using NUnit.Framework;
using static Frank.API.WebDevelopers.DTO.ResponseBuilders;

namespace Frank.EndToEndTests
{
    public class FrankTestHarnessConnectionTest
    {
        [Test]
        public void CanMakeTestRequestAndRespondWith404()
        {
            ITestWebApplication webApplication = Server.Configure().StartTesting();

            var response = webApplication.Execute(new Request());

            response.Status.Should().Be(404);
        }

        [Test]
        public void CanRouteToARoute()
        {
            ITestWebApplication webApplication = Server
                .Configure()
                .OnRequest(
                    c => c.Get("/").To(request => Ok().WithBody(new {a = 123}))
                ).StartTesting();

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

            webApplication.Execute(new Request());
            webApplication.Execute(new Request());

            webApplication.Stop();
            i.Should().Be(2);
        }

        [Test]
        public void CanExecuteBeforeAndAfterHandlers()
        {
            var customContext = new LifecycleHooksSpy();
            var webApplication = Server.Configure()
                .Before(() =>
                {
                    customContext.Before();
                    return customContext;
                })
                .After(context => { context.After(); })
                .OnRequest(((configurer, context) =>
                {
                    context.Request();
                    configurer.Get("/").To(() =>
                    {
                        context.RouteHandler();
                        throw new Exception();
                    });
                }))
                .StartTesting();

            webApplication.Execute(new Request());

            customContext._list.Should().ContainInOrder(
                "before", "request", "route-handler", "after"
            );
        }
    }
}