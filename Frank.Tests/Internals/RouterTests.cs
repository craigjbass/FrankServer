using System;
using FluentAssertions;
using Frank.API.WebDevelopers.DTO;
using Frank.Internals;
using NUnit.Framework;

namespace Frank.Tests.Internals
{
    public class RouterTests
    {
        private RequestRouter _requestRouter;

        private static Response CreateRandomResponse()
        {
            var bytes = new Span<Byte>();
            new Random().NextBytes(bytes);
            var response = new Response
            {
                Status = new Random().Next(),
                Body = bytes.ToArray()
            };
            return response;
        }

        [SetUp]
        public void SetUp()
        {
            _requestRouter = new RequestRouter();
        }

        [Test]
        public void WhenNoRoutesDoesNotRoute()
        {
            _requestRouter.CanRoute("/").Should().BeFalse();
        }

        [Test]
        public void WhenCanNotRouteThrowsExceptionOnRouting()
        {
            Action action = () => _requestRouter.Route(new Request());
            action.Should().Throw<Unroutable>();
        }

        [Test]
        [TestCase("/", "/", true)]
        [TestCase("/a-path", "/a-path", true)]
        [TestCase("/a-path", "/a-different-path", false)]
        public void CanCheckRoutabilityWhenThereAreRoutes(
            string route,
            string expectedRoute, bool expectedRoutability
        )
        {
            _requestRouter.Get(route).To(() => new Response());
            _requestRouter.CanRoute(expectedRoute).Should().Be(expectedRoutability);
        }

        [Test]
        [TestCase("/")]
        [TestCase("/a-route")]
        public void CanGetResponseForRoute(string route)
        {
            var response = CreateRandomResponse();
            _requestRouter.Get(route).To(() => response);
            _requestRouter.Route(new Request {Path = route}).Should().Be(response);
        }

        [Test]
        public void CanRouteAPostRequest()
        {
            var response = CreateRandomResponse();
            _requestRouter.Post("/any").To( _ => response);
            _requestRouter.Route(new Request {Path = "/any", Method = "POST", Body = "yo"}).Should().Be(response);
        }

        [Test]
        public void CanPassRequestToRoute()
        {
            Request? myRequest = null;
            _requestRouter.Get("/hello").To(request =>
            {
                myRequest = request;
                return CreateRandomResponse();
            });


            _requestRouter.Route(new Request {Path = "/hello"});


            myRequest?.Path.Should().Be("/hello");
        }
    }
}