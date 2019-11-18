using System.Collections.Generic;

namespace Frank.EndToEndTests
{
    class LifecycleHooksSpy
    {
        public readonly List<string> OrderThatMethodsWereCalled = new List<string>();

        public void Before()
        {
            OrderThatMethodsWereCalled.Add("before");
        }

        public void After()
        {
            OrderThatMethodsWereCalled.Add("after");
        }

        public void Request()
        {
            OrderThatMethodsWereCalled.Add("request");
        }

        public void RouteHandler()
        {
            OrderThatMethodsWereCalled.Add("route-handler");
        }
    }
}