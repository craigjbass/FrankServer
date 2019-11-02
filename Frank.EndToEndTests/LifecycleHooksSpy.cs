using System.Collections.Generic;

namespace Frank.EndToEndTests
{
    class LifecycleHooksSpy
    {
        public List<string> _list = new List<string>();

        public void Before()
        {
            _list.Add("before");
        }

        public void After()
        {
            _list.Add("after");
        }

        public void Request()
        {
            _list.Add("request");
        }

        public void RouteHandler()
        {
            _list.Add("route-handler");
        }
    }
}