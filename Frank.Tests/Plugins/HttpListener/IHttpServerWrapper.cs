namespace Frank.Tests.Plugins.HttpListener
{
    public interface IHttpServerWrapper
    {
        void Start();
        void Stop();
        string Host { get; }
    }
}