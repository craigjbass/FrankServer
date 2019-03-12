using System;
using System.Net;
using Frank.DTO;

namespace Frank
{
    internal class HttpListenerServer : WebApplication.IServer
    {
        private HttpListener _httpListener;
        private bool _running;

        public void Start(ListenOn[] listenOns)
        {
            _running = true;
            _httpListener = new HttpListener();
            foreach (var listenOn in listenOns)
            {
                _httpListener.Prefixes.Add($"http://{listenOn.HostName}:{listenOn.Port}/");
            }
            _httpListener.Start();
        }

        class ResponseWriter : WebApplication.IResponseWriter
        {
            private readonly HttpListenerResponse _response;

            public ResponseWriter(HttpListenerResponse response)
            {
                _response = response;
            }

            public void WriteResponse(Response response)
            {
                _response.StatusCode = response.Status;
                _response.OutputStream.Write(response.Body);
            }

            public void Flush()
            {
                _response.Close();
            }
        }
        
        public void RegisterRequestHandler(Action<Request, WebApplication.IResponseWriter> processRequest)
        {
            IAsyncResult context;
            do
            {
                context = _httpListener.BeginGetContext(ar =>
                {
                    HttpListenerContext c = ((HttpListener) ar.AsyncState).EndGetContext(ar);
                    var response = c.Response;
                    var request = c.Request;
                    processRequest(new Request()
                    {
                        Path = request.RawUrl
                    }, new ResponseWriter(response));
                    
                }, _httpListener);
            } while (_running && context.AsyncWaitHandle.WaitOne());
        }

        public void Stop()
        {
            _running = false;
            _httpListener.Stop();
        }
    }
}