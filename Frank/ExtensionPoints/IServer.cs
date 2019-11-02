using System;
using Frank.API.PluginDevelopers;
using Frank.API.WebDevelopers.DTO;

namespace Frank.ExtensionPoints
{
    public interface IServer
    {
        void Start();
        void RegisterRequestHandler(Action<Request, IResponseBuffer> requestHandler);
        void Stop();
    }
}