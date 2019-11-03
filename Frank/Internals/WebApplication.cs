using System;
using System.Collections.Generic;
using System.Linq;
using Frank.API.PluginDevelopers;
using Frank.API.WebDevelopers;
using Frank.API.WebDevelopers.DTO;
using Frank.ExtensionPoints;
using static Frank.API.WebDevelopers.DTO.ResponseBuilders;

namespace Frank.Internals
{
    internal class WebApplication : IWebApplication
    {
        private readonly IServer _server;
        private readonly List<Exception> _exceptions = new List<Exception>();
        private readonly Action<IRouteConfigurer, object> _onRequest;
        private readonly Func<object> _onBefore;
        private readonly Action<object> _onAfter;

        internal interface IRequestRouter
        {
            bool CanRoute(string url);

            Response Route(
                Request request
            );
        }

        public WebApplication(
            IServer server,
            Action<IRouteConfigurer, object> onRequest,
            Func<object> onBefore,
            Action<object> onAfter)
        {
            _server = server;
            _onRequest = onRequest;
            _onBefore = onBefore;
            _onAfter = onAfter;
        }

        public void Start()
        {
            _server.Start();
            _server.RegisterRequestHandler(ProcessRequest);
        }

        private void ProcessRequest(Request request, IResponseBuffer responseBuffer)
        {
            var context = _onBefore();
            var router = new RequestRouter();
            _onRequest(router, context);

            try
            {
                if (router.CanRoute(request.Path))
                {
                    var actualResponse = router.Route(request);
                    responseBuffer.SetContentsOfBufferTo(actualResponse);
                }
                else
                {
                    responseBuffer.SetContentsOfBufferTo(NewResponseWithStatus(404));
                }
            }
            catch (Exception e)
            {
                _exceptions.Add(e);
                responseBuffer.SetContentsOfBufferTo(
                    NewResponseWithStatus(500).BodyFromString(_exceptions.First().ToString())
                );
            }
            finally
            {
                responseBuffer.Flush();
                _onAfter(context);
            }
        }

        public void Stop()
        {
            _server.Stop();

            if (_exceptions.Any())
            {
                var exception = new Exception();
                exception.Data.Add("Exceptions", _exceptions);
                throw exception;
            }
        }
    }
}