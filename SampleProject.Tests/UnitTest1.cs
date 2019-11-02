using System.Text;
using System.Text.Json.Serialization;
using Frank.API.WebDevelopers.DTO;
using Newtonsoft.Json;
using NUnit.Framework;

namespace SampleProject.Tests
{
    public class Tests
    {
        [Test]
        public void CanCheckJsonResponseBodyContainsSuccess()
        {
            var testWebApplication = new SampleProjectApp().CreateApplication().StartTesting();

            var response = testWebApplication.Execute(new Request()
            {
                Path = "/test",
                Method = "GET"
            });

            var body = Encoding.UTF8.GetString(response.Body);
            dynamic deserializedBody = JsonConvert.DeserializeObject(body);
            
            Assert.AreEqual(true, (bool)deserializedBody.Success);
        }
    }
}