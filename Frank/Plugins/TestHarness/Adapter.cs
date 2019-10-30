using Frank.API.PluginDevelopers;
using Frank.API.WebDevelopers;
using Frank.API.WebDevelopers.DTO;

namespace Frank.Plugins.TestHarness
{
    internal class Adapter : ITestWebApplication
    {
        private readonly IWebApplication _application;
        private readonly TestHarnessServer _testHarness;

        public Adapter(IWebApplication application, TestHarnessServer testHarness)
        {
            _application = application;
            _testHarness = testHarness;
        }

        public void Start() => _application.Start();

        public void Stop() => _application.Stop();

        public Response Execute(Request request) => _testHarness.Execute(request);
    }

    internal class TestResponseBuffer : IResponseBuffer
    {
        private Response _response;

        public void SetContentsOfBufferTo(Response response)
        {
            _response = response;
        }

        public void Flush()
        {
            
        }

        public Response GetContents()
        {
            return _response;
        }
    }
}