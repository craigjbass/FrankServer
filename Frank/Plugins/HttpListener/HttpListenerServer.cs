using System;
using System.Collections.Specialized;
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

        public void Start(int port)
        {
            _running = true;
            _httpListener = new System.Net.HttpListener();

            _httpListener.Prefixes.Add($"http://+:{port}/");

            _httpListener.Start();
        }

        public void RegisterRequestHandler(Action<Request, IResponseBuffer> processRequest)
        {
            IAsyncResult context;
            do
            {
                context = _httpListener.BeginGetContext(ar =>
                {
                    HttpListenerContext c = ((System.Net.HttpListener) ar.AsyncState).EndGetContext(ar);
                    Console.WriteLine($"REQUEST {c.Request.Url}");

                    processRequest(
                        new RequestConverter(c.Request).Convert(),
                        new ResponseBuffer(c.Response)
                    );
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