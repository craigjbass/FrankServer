using System;
using System.Net;
using Frank.ExtensionPoints;
using Frank.API.PluginDevelopers;
using Frank.API.WebDevelopers.DTO;

namespace Frank.Plugins.HttpListener
{
    internal class HttpListenerServer : IServer
    {
        private System.Net.HttpListener _httpListener;
        private int _port;

        public HttpListenerServer(int port)
        {
            _port = port;
        }

        public void Start()
        {
            _httpListener = new System.Net.HttpListener();

            _httpListener.Prefixes.Add($"http://+:{_port}/");

            _httpListener.Start();
        }

        public void RegisterRequestHandler(Action<Request, IResponseBuffer> requestHandler)
        {
            _httpListener.BeginGetContext(TryToPassRequestToHandler(requestHandler), _httpListener);
        }

        private AsyncCallback TryToPassRequestToHandler(Action<Request, IResponseBuffer> requestHandler)
        {
            return ar =>
            {
                try
                {
                    HttpListenerContext c = ((System.Net.HttpListener) ar.AsyncState).EndGetContext(ar);

                    requestHandler(
                        new RequestConverter(c.Request).Convert(),
                        new ResponseBuffer(c.Response)
                    );

                    ar.AsyncWaitHandle.WaitOne();

                    _httpListener.BeginGetContext(TryToPassRequestToHandler(requestHandler), _httpListener);
                }
                catch (ObjectDisposedException)
                {
                }
                catch (HttpListenerException exception)
                {
                    var notAnAbortedConnectionException = exception.ErrorCode != 995;
                    if (notAnAbortedConnectionException) throw;
                }
            };
        }

        public void Stop()
        {
            _httpListener.Stop();
        }
    }
}