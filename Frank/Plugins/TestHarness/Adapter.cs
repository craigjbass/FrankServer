using System.Text;
using Frank.API.PluginDevelopers;
using Frank.API.WebDevelopers;
using Frank.API.WebDevelopers.DTO;
using Newtonsoft.Json;

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

        public ITestResponse Execute(Request request) => _testHarness.Execute(request);
    }

    internal class TestResponseBuffer : IResponseBuffer, ITestResponse
    {
        private Response _response;

        public void SetContentsOfBufferTo(Response response)
        {
            _response = response;
        }

        public void Flush()
        {
            
        }

        public int Status => _response.Status;

        public string Body => Encoding.UTF8.GetString(_response.Body);
        public dynamic DeserializeBody() => JsonConvert.DeserializeObject(Body);
    }
}