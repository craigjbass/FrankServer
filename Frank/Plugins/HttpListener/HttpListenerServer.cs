using System;
using System.Net;
using System.Runtime.CompilerServices;
using Frank.ExtensionPoints;
using Frank.API.PluginDevelopers;
using Frank.API.WebDevelopers.DTO;

namespace Frank.Plugins.HttpListener
{
    internal class HttpListenerServer : IServer
    {
        private System.Net.HttpListener _httpListener;
        private bool _running;

        public void Start(ListenOn[] listenOns)
        {
            _running = true;
            _httpListener = new System.Net.HttpListener();
            foreach (var listenOn in listenOns)
            {
                _httpListener.Prefixes.Add($"http://{listenOn.HostName}:{listenOn.Port}/");
            }
            _httpListener.Start();
        }

        class ResponseBuffer : IResponseBuffer
        {
            private readonly HttpListenerResponse _response;

            public ResponseBuffer(HttpListenerResponse response)
            {
                _response = response;
            }

            public void SetContentsOfBufferTo(Response response)
            {
                _response.StatusCode = response.Status;
                _response.OutputStream.Write(response.Body);
            }

            public void Flush()
            {
                _response.Close();
            }
        }
        
        public void RegisterRequestHandler(Action<Request, IResponseBuffer> processRequest)
        {
            IAsyncResult context;
            do
            {
                context = _httpListener.BeginGetContext(ar =>
                {
                    HttpListenerContext c = ((System.Net.HttpListener) ar.AsyncState).EndGetContext(ar);
                    var response = c.Response;
                    var request = c.Request;
                    processRequest(new Request
                    {
                        Path = request.RawUrl
                    }, new ResponseBuffer(response));
                    
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