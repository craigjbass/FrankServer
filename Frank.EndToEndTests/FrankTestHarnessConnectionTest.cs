using System;
using System.Text;
using FluentAssertions;
using Frank.API.WebDevelopers;
using Frank.API.WebDevelopers.DTO;
using NUnit.Framework;
using static Frank.API.WebDevelopers.DTO.RequestConstructors;
using static Frank.API.WebDevelopers.DTO.ResponseConstructors;

namespace Frank.EndToEndTests
{
    public class FrankTestHarnessConnectionTest
    {
        [Test]
        public void CanMakeTestRequestAndRespondWith404()
        {
            ITestWebApplication webApplication = Server.Configure().StartTesting();

            var response = webApplication.Execute(Get());

            response.Status.Should().Be(404);
        }

        [Test]
        public void CanRouteToARoute()
        {
            ITestWebApplication webApplication = Server
                .Configure()
                .OnRequest(
                    c =>
                    {
                        c.Get("/").To(request => Ok().WithJsonBody(new {a = 123}));
                        c.Post("/post").To(request => Ok().WithJsonBody(request));
                    }).StartTesting();

            {
                var response = webApplication.Execute(Get());
                response.Status.Should().Be(200);
                response.Body.Should().Be("{\"a\":123}");
            }

            {
                var response = webApplication.Execute(Post().WithPath("/post"));
                response.Status.Should().Be(200);
                response.Body.Should().Be(
                    "{\"Path\":\"/post\",\"Body\":\"\",\"QueryParameters\":{},\"Headers\":{},\"Method\":\"POST\"}"
                );
            }

            webApplication.Stop();
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

            webApplication.Execute(Get());
            webApplication.Execute(Get());

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

            webApplication.Execute(Get());

            customContext._list.Should().ContainInOrder(
                "before", "request", "route-handler", "after"
            );
        }
    }
}