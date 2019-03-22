using FluentAssertions;
using Frank.ExtensionPoints;
using Frank.Plugins.HttpListener;
using NUnit.Framework;
using RestSharp;

namespace Frank.Tests.Plugins.HttpListener
{
    [SingleThreaded]
    public class HttpListenerServerTests
    {
        private HttpListenerServer _listener;
        private IRestResponse _response;

        [SetUp]
        public void SetUp()
        {
            _listener = new HttpListenerServer();
        }
        
        [TearDown]
        public void TearDown()
        {
            _listener.Stop();
        }

        private void MakeGetRequest(string url, string path)
        {
            var request = new RestRequest(path, Method.GET);
            request.Timeout = 100;
            var client = new RestClient(url);
            _response = client.Execute(request);
        }

        private void AssertTimedOut()
        {
            _response.ResponseStatus.Should().Be(ResponseStatus.TimedOut);
        }

        private void AssertStatusCodeIs(int expected)
        {
            _response.StatusCode.Should().Be(expected);
        }

        [Test]
        public void RespondsWith404OnAnyUnregisteredHostname()
        {
            _listener.Start(new[] {new ListenOn {HostName = "127.0.0.1", Port = 8021}});
            MakeGetRequest("http://localhost:8021/", "/");    
            AssertStatusCodeIs(404);
        }

        [Test]
        public void TimesOutWhenNoRequestProcessorRegistered()
        {   
            _listener.Start(new[] {new ListenOn {HostName = "127.0.0.1", Port = 8020}});
            MakeGetRequest("http://127.0.0.1:8020/", "/");
            AssertTimedOut();   
        }
    }
}