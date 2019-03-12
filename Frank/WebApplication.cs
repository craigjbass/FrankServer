using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Frank.DTO;

[assembly: InternalsVisibleTo("Frank.Tests")]

namespace Frank
{
    internal class WebApplication : IWebApplication
    {
        private readonly IServer _server;
        private readonly IRouter _router;
        private readonly List<Exception> _exceptions = new List<Exception>();

        public interface IServer
        {
            void Start(ListenOn[] listenOns);
            void RegisterRequestHandler(Action<Request, IResponseWriter> processRequest);
            void Stop();
        }

        public interface IResponseWriter
        {
            void WriteResponse(Response response);
            void Flush();
        }

        public interface IRouter
        {
            bool CanRoute(string url);

            Response Route(
                Request request
            );
        }

        public WebApplication(IServer server, IRouter router)
        {
            _server = server;
            _router = router;
        }

        public void Start()
        {
            _server.Start(new[] {new ListenOn {HostName = "127.0.0.1", Port = 8019}});
            new Task(() => _server.RegisterRequestHandler(ProcessRequest)).Start();
        }

        private void ProcessRequest(Request request, IResponseWriter responseWriter)
        {
            try
            {
                if (_router.CanRoute(request.Path))
                {
                    var actualResponse = _router.Route(request);
                    responseWriter.WriteResponse(actualResponse);
                }
                else
                {
                    responseWriter.WriteResponse(new Response
                    {
                        Status = 404
                    });
                }
            }
            catch (Exception e)
            {
                _exceptions.Add(e);
                responseWriter.WriteResponse(new Response
                {
                    Status = 500
                }.BodyFromString(_exceptions.First().ToString()));
            }
            finally
            {
                responseWriter.Flush();
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