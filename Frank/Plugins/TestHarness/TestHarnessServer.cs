using System;
using Frank.API.PluginDevelopers;
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