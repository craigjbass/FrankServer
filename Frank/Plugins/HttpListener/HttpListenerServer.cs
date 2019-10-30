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
            
            _httpListener.BeginGetContext(Callback(processRequest), _httpListener);
        }

        private AsyncCallback Callback(Action<Request, IResponseBuffer> processRequest)
        {
            return ar =>
            {
                HttpListenerContext c = ((System.Net.HttpListener) ar.AsyncState).EndGetContext(ar);

                processRequest(
                    new RequestConverter(c.Request).Convert(),
                    new ResponseBuffer(c.Response)
                );

                _httpListener.BeginGetContext(Callback(processRequest), _httpListener);
            };
        }

        public void Stop()
        {
            _running = false;
            _httpListener.Stop();
        }
    }
}