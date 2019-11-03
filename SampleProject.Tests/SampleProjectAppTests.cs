using Frank.API.WebDevelopers.DTO;
using NUnit.Framework;

namespace SampleProject.Tests
{
    public class SampleProjectAppTests
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
            
            var deserializedBody = response.DeserializeBody();
            
            Assert.AreEqual(true, (bool)deserializedBody.Success);
        }
    }
}