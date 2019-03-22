using System;
using Frank.API.PluginDevelopers;
using Frank.API.WebDevelopers.DTO;

namespace Frank.ExtensionPoints
{
    public interface IServer
    {
        void Start(ListenOn[] listenOns);
        void RegisterRequestHandler(Action<Request, IResponseBuffer> processRequest);
        void Stop();
    }

    public struct ListenOn
    {
        public string HostName;
        public int Port;
    }
}