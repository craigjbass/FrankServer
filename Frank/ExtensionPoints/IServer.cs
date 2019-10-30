using System;
using Frank.API.PluginDevelopers;
using Frank.API.WebDevelopers.DTO;

namespace Frank.ExtensionPoints
{
    public interface IServer
    {
        void Start(int port);
        void RegisterRequestHandler(Action<Request, IResponseBuffer> processRequest);
        void Stop();
    }
}