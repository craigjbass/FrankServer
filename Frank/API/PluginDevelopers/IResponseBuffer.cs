using Frank.API.WebDevelopers.DTO;

namespace Frank.API.PluginDevelopers
{
    public interface IResponseBuffer
    {
        void SetContentsOfBufferTo(Response response);
        void Flush();
    }
}