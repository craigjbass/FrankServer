using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
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
                    Console.WriteLine($"REQUEST {request.Url}");

                    processRequest(new Request
                    {
                        Path = Path(request),
                        Body = "",
                        QueryParameters = QueryParameters(request),
                        Headers = Headers(request)
                    }, new ResponseBuffer(response));
                }, _httpListener);
            } while (_running && context.AsyncWaitHandle.WaitOne());
        }

        private static Dictionary<string, string> Headers(HttpListenerRequest request)
        {
            return new Dictionary<string, string>(
                request.Headers.AllKeys.ToDictionary(k => k, k => request.Headers[k]),
                StringComparer.OrdinalIgnoreCase
            );
        }

        private static string Path(HttpListenerRequest request)
        {
            return request.RawUrl.Substring(
                0,
                QuestionMarkLocation(request) ?? request.RawUrl.Length
            );
        }

        private static Dictionary<string, string> QueryParameters(HttpListenerRequest request)
        {
            return request.QueryString.AllKeys.ToDictionary(k => k, k => request.QueryString[k]);
        }

        private static int? QuestionMarkLocation(HttpListenerRequest request)
        {
            var markLocation = request.RawUrl.IndexOf('?');
            if (markLocation < 0) return null;
            return markLocation;
        }

        public void Stop()
        {
            _running = false;
            _httpListener.Stop();
        }
    }
}