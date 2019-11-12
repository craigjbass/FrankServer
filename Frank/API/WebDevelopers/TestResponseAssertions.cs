using System;
using Newtonsoft.Json;

namespace Frank.API.WebDevelopers
{
    public static class TestResponseAssertions
    {
        public class FrankAssertionException : Exception
        {
            public FrankAssertionException(string message) : base(message)
            {
            }
        }

        public static void AssertIsNotFound(this ITestResponse response) => response.AssertStatusIs(404);

        public static void AssertIsOk(this ITestResponse response) => response.AssertStatusIs(200);

        public static void AssertStatusIs(this ITestResponse response, int expectedStatus)
        {
            if(response.Status != expectedStatus) 
                throw new FrankAssertionException($"Expected {expectedStatus}, found {response.Status}");
        }

        public static void AssertJsonBodyMatches(this ITestResponse response, object expected)
        {
            var expectedString = JsonConvert.SerializeObject(expected);
            if(response.Body != expectedString)
                throw new FrankAssertionException($"Expected {expectedString}, found {response.Body}");
        }
    }
}