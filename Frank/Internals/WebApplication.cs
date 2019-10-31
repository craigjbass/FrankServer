using System;
using System.Collections.Generic;
using System.Linq;
using Frank.API.PluginDevelopers;
using Frank.API.WebDevelopers;
using Frank.API.WebDevelopers.DTO;
using Frank.ExtensionPoints;

namespace Frank.Internals
{
    internal class WebApplication : IWebApplication
    {
        private readonly IServer _server;
        private readonly List<Exception> _exceptions = new List<Exception>();
        private readonly Action<IRouteConfigurer> _onRequest;

        internal interface IRequestRouter
        {
            bool CanRoute(string url);

            Response Route(
                Request request
            );
        }

        public WebApplication(IServer server, Action<IRouteConfigurer> onRequest)
        {
            _server = server;
            _onRequest = onRequest;
        }

        public void Start()
        {
            _server.Start();
            _server.RegisterRequestHandler(ProcessRequest);
        }

        private void ProcessRequest(Request request, IResponseBuffer responseBuffer)
        {
            var router = new RequestRouter();
            _onRequest(router);
            try
            {
                if (router.CanRoute(request.Path))
                {
                    var actualResponse = router.Route(request);
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