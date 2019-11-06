using Frank.API.WebDevelopers.DTO;
using NUnit.Framework;
using static Frank.API.WebDevelopers.DTO.RequestConstructors;

namespace SampleProject.Tests
{
    public class SampleProjectAppTests
    {
        [Test]
        public void CanCheckJsonResponseBodyContainsSuccess()
        {
            var testWebApplication = new SampleProjectApp().CreateApplication().StartTesting();

            var response = testWebApplication.Execute(Get().WithPath("/test"));
            
            var deserializedBody = response.DeserializeBody();
            
            Assert.AreEqual(true, (bool)deserializedBody.Success);
        }
    }
}