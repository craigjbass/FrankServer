using FluentAssertions;
using Frank.API.PluginDevelopers;
using Frank.API.WebDevelopers;
using Frank.API.WebDevelopers.DTO;
using Frank.Internals;
using NUnit.Framework;

namespace Frank.Tests.Internals
{
    public class TestHarnessTests : IWebApplication
    {
        private bool _startCalled;
        private bool _stopCalled;
        private Request? _arg1;
        private Response _nextResponse;

        public void Start() => _startCalled = true;

        public void Stop() => _stopCalled = true;

        [SetUp]
        public void SetUp()
        {
            _startCalled = false;
            _stopCalled = false;
            _arg1 = null;
            _nextResponse = new Response{Status = 404};
        }

        private void ProcessRequest(Request arg1, IResponseBuffer arg2)
        {
            _arg1 = arg1;
            arg2.SetContentsOfBufferTo(_nextResponse);
            arg2.Flush();
        }

        [Test]
        public void CanProxyStartAndStop()
        {
            var testHarness = new TestHarness(this, new TestHarnessServer());
            testHarness.Start();
            testHarness.Stop();

            _startCalled.Should().BeTrue();
            _stopCalled.Should().BeTrue();
        }

        [Test]
        public void CanCallRequestHandler()
        {
            var testHarnessServer = new TestHarnessServer();
            testHarnessServer.RegisterRequestHandler(ProcessRequest);
            var testHarness = new TestHarness(this, testHarnessServer);

            var response = testHarness.Execute(new Request
            {
                Method = "GET"
            });

            _arg1.Value.Method.Should().Be("GET");
            response.Status.Should().Be(404);

        }
        
        [Test]
        public void CanCallRequestHandler2()
        {
            var testHarnessServer = new TestHarnessServer();
            testHarnessServer.RegisterRequestHandler(ProcessRequest);
            var testHarness = new TestHarness(this, testHarnessServer);

            _nextResponse = new Response()
            {
                Status = 200
            };
            var response = testHarness.Execute(new Request
            {
                Method = "POST"
            });

            _arg1.Value.Path.Should().Be("/");
            _arg1.Value.Method.Should().Be("POST");
            response.Status.Should().Be(200);
        }
    }
}