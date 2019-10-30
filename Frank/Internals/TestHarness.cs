using System;
using Frank.API.PluginDevelopers;
using Frank.API.WebDevelopers;
using Frank.API.WebDevelopers.DTO;
using Frank.ExtensionPoints;

namespace Frank.Internals
{
    internal class TestHarness : ITestWebApplication
    {
        private readonly IWebApplication _application;
        private readonly TestHarnessServer _testHarness;

        public TestHarness(IWebApplication application, TestHarnessServer testHarness)
        {
            _application = application;
            _testHarness = testHarness;
        }

        public void Start() => _application.Start();

        public void Stop() => _application.Stop();

        public Response Execute(Request request) => _testHarness.Execute(request);
    }

    internal class TestResponseBuffer : IResponseBuffer
    {
        private Response _response;

        public void SetContentsOfBufferTo(Response response)
        {
            _response = response;
        }

        public void Flush()
        {
            
        }

        public Response GetContents()
        {
            return _response;
        }
    }

    internal class TestHarnessServer : IServer
    {
        private Action<Request, IResponseBuffer> _processRequest;

        public void RegisterRequestHandler(Action<Request, IResponseBuffer> processRequest)
        {
            _processRequest = processRequest;
        }

        public Response Execute(Request request)
        {
            request.Path = "/";
            var buffer = new TestResponseBuffer();
            _processRequest(request, buffer);
            return buffer.GetContents();
        }

        public void Start()
        {
            
        }

        public void Stop()
        {
            
        }
    }
}