using System.Collections.Generic;

namespace Frank.EndToEndTests
{
    class LifecycleHooksSpy
    {
        public readonly List<string> List = new List<string>();

        public void Before()
        {
            List.Add("before");
        }

        public void After()
        {
            List.Add("after");
        }

        public void Request()
        {
            List.Add("request");
        }

        public void RouteHandler()
        {
            List.Add("route-handler");
        }
    }
}