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
            _requestRouter.Get(route, () => new Response());
            _requestRouter.CanRoute(expectedRoute).Should().Be(expectedRoutability);
        }

        [Test]
        [TestCase("/")]
        [TestCase("/a-route")]
        public void CanGetResponseForRoute(string route)
        {
            var response = CreateRandomResponse();
            _requestRouter.Get(route, () => response);
            _requestRouter.Route(new Request {Path = route}).Should().Be(response);
        }
    }
}