using System.Net;
using Frank.API.PluginDevelopers;
using Frank.API.WebDevelopers.DTO;

namespace Frank.Plugins.HttpListener
{
    internal class ResponseBuffer : IResponseBuffer
    {
        private readonly HttpListenerResponse _response;

        public ResponseBuffer(HttpListenerResponse response)
        {
            _response = response;
        }

        public void SetContentsOfBufferTo(Response response)
        {
            _response.StatusCode = response.Status;
            _response.OutputStream.Write(response.Body);
        }

        public void Flush()
        {
            _response.Close();
        }
    }
}