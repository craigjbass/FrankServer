using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frank.API.PluginDevelopers;
using Frank.API.WebDevelopers;
using Frank.API.WebDevelopers.DTO;
using Frank.ExtensionPoints;

namespace Frank.Internals
{
    internal class WebApplication : IWebApplication
    {
        private readonly IServer _server;
        private readonly IRequestRouter _requestRouter;
        private readonly List<Exception> _exceptions = new List<Exception>();

        internal interface IRequestRouter
        {
            bool CanRoute(string url);

            Response Route(
                Request request
            );
        }

        public WebApplication(IServer server, IRequestRouter requestRouter)
        {
            _server = server;
            _requestRouter = requestRouter;
        }

        public void Start()
        {
            _server.Start(new[] {new ListenOn {HostName = "127.0.0.1", Port = 8019}});
            new Task(() => _server.RegisterRequestHandler(ProcessRequest)).Start();
        }

        private void ProcessRequest(Request request, IResponseBuffer responseBuffer)
        {
            try
            {
                if (_requestRouter.CanRoute(request.Path))
                {
                    var actualResponse = _requestRouter.Route(request);
                    responseBuffer.SetContentsOfBufferTo(actualResponse);
                }
                else
                {
                    responseBuffer.SetContentsOfBufferTo(new Response
                    {
                        Status = 404
                    });
                }
            }
            catch (Exception e)
            {
                _exceptions.Add(e);
                responseBuffer.SetContentsOfBufferTo(new Response
                {
                    Status = 500
                }.BodyFromString(_exceptions.First().ToString()));
            }
            finally
            {
                responseBuffer.Flush();
            }
        }

        public void Stop()
        {
            if (_exceptions.Any())
            {
                var exception = new Exception();
                exception.Data.Add("Exceptions", _exceptions);
                throw exception;
            }

            _server.Stop();
        }
    }
}