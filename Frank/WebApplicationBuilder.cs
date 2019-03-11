using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Frank
{
    public class WebApplicationBuilder
    {
        private RouteConfigurer _routerConfigurer;

        public void WithRoutes(Action<IRouteConfigurer> action)
        {
            _routerConfigurer = new RouteConfigurer();
            action(_routerConfigurer);
        }

        public void ListenOn(string ip, string port)
        {
        }

        public IWebApplication Build()
        {
            return new WebApplication(_routerConfigurer ?? new RouteConfigurer());
        }

        internal class RouteConfigurer : IRouteConfigurer, WebApplication.IRouter
        {
            private readonly Dictionary<string, Func<int>> _routes = new Dictionary<string, Func<int>>();

            public void Get(string path, Func<int> func)
            {
                _routes.Add(path, func);
            }

            public bool CanRoute(string url)
            {
                return _routes.ContainsKey(url);
            }

            public void Route(IRequestifier requestifier, IResponsifier responsifier)
            {
                responsifier.StatusCode = _routes[requestifier.Path]();
            }

            public interface IRequestifier
            {
                string Path { get; }
            }

            public interface IResponsifier
            {
                int StatusCode { set; }
            }
        }
    }

    class WebApplication : IWebApplication
    {
        private readonly IRouter _router;
        private HttpListener _httpListener;
        private readonly List<Exception> _exceptions = new List<Exception>();
        private bool _running;

        public WebApplication(IRouter router)
        {
            _router = router;
        }

        internal interface IRouter
        {
            bool CanRoute(string url);

            void Route(
                WebApplicationBuilder.RouteConfigurer.IRequestifier requestifier,
                WebApplicationBuilder.RouteConfigurer.IResponsifier responsifier
            );
        }

        public void Start()
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add("http://127.0.0.1:8019/");
            _httpListener.Start();
            _running = true;

            new Task(() =>
            {
                IAsyncResult context;
                do
                {
                    context = _httpListener.BeginGetContext(ProcessRequest, _httpListener);
                } while (_running && context.AsyncWaitHandle.WaitOne());
            }).Start();
        }

        public void Stop()
        {
            if (_exceptions.Any())
            {
                var exception = new Exception();
                exception.Data.Add("Exceptions", _exceptions);
                throw exception;
            }

            _running = false;
            _httpListener.Stop();
        }

        class ContextAdapter : 
            WebApplicationBuilder.RouteConfigurer.IResponsifier, 
            WebApplicationBuilder.RouteConfigurer.IRequestifier
        {
            private readonly HttpListenerRequest _request;
            private readonly HttpListenerResponse _response;

            public ContextAdapter(HttpListenerRequest request, HttpListenerResponse response)
            {
                _request = request;
                _response = response;
            }

            public int StatusCode
            {
                set => _response.StatusCode = value;
            }
            public string Path
            {
                get => _request.RawUrl;
            }
        }

        private void ProcessRequest(IAsyncResult ar)
        {
            var context = ((HttpListener) ar.AsyncState).EndGetContext(ar);
            var response = context.Response;
            var request = context.Request;
            try
            {
                if (_router.CanRoute(request.RawUrl))
                {
                    var contextAdapter = new ContextAdapter(request, response);
                    _router.Route(contextAdapter, contextAdapter);
                }
                else
                {
                    response.StatusCode = 404;
                }
            }
            catch (Exception e)
            {
                _exceptions.Add(e);
                response.StatusCode = 500;
                response.OutputStream.Write(
                    Encoding.UTF8.GetBytes(_exceptions.First().ToString())
                );
            }
            finally
            {
                response.Close();
            }
        }
    }

    public interface IWebApplication
    {
        void Start();
        void Stop();
    }

    public interface IRouteConfigurer
    {
        void Get(string path, Func<int> func);
    }
}