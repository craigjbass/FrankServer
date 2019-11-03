using System;
using Frank.API.PluginDevelopers;
using Frank.API.WebDevelopers;
using Frank.API.WebDevelopers.DTO;
using Frank.ExtensionPoints;

namespace Frank.Plugins.TestHarness
{
    internal class TestHarnessServer : IServer
    {
        private Action<Request, IResponseBuffer> _processRequest;

        public void RegisterRequestHandler(Action<Request, IResponseBuffer> requestHandler)
        {
            _processRequest = requestHandler;
        }

        public ITestResponse Execute(Request request)
        {
            if(request.Path == null) request.Path = "/";
            var buffer = new TestResponseBuffer();
            _processRequest(request, buffer);
            return buffer;
        }

        public void Start()
        {
            
        }

        public void Stop()
        {
            
        }
    }
}