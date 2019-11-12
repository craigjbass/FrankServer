using FluentAssertions;
using Frank.API.PluginDevelopers;
using Frank.API.WebDevelopers;
using Frank.API.WebDevelopers.DTO;
using Frank.Plugins.TestHarness;
using NUnit.Framework;

namespace Frank.Tests.Plugins.TestHarness
{
    public class TestHarnessTests : IWebApplication
    {
        private bool _startCalled;
        private bool _stopCalled;
        private Request? _lastRequest;
        private Response _nextResponse;

        public void Start() => _startCalled = true;

        public void Stop() => _stopCalled = true;

        private Request LastRequest() => _lastRequest.Value;

        [SetUp]
        public void SetUp()
        {
            _startCalled = false;
            _stopCalled = false;
            _lastRequest = null;
            _nextResponse = new Response{Status = 404};
        }

        private void ProcessRequest(Request request, IResponseBuffer responseBuffer)
        {
            _lastRequest = request;
            responseBuffer.SetContentsOfBufferTo(_nextResponse);
            responseBuffer.Flush();
        }

        [Test]
        public void CanProxyStartAndStop()
        {
            var testHarness = new Adapter(this, new TestHarnessServer());
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
            var testHarness = new Adapter(this, testHarnessServer);

            var response = testHarness.Execute(new Request
            {
                Method = "GET"
            });
            
            LastRequest().Method.Should().Be("GET");
            response.AssertIsNotFound();
        }

        [Test]
        public void CanCallRequestHandler2()
        {
            var testHarnessServer = new TestHarnessServer();
            testHarnessServer.RegisterRequestHandler(ProcessRequest);
            var testHarness = new Adapter(this, testHarnessServer);

            _nextResponse = new Response()
            {
                Status = 200
            };
            var response = testHarness.Execute(new Request
            {
                Method = "POST"
            });

            LastRequest().Path.Should().Be("/");
            LastRequest().Method.Should().Be("POST");
            response.AssertIsOk();
        }
    }
}