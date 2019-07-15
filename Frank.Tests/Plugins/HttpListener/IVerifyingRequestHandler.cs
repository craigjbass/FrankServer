using Frank.API.WebDevelopers.DTO;

namespace Frank.Tests.Plugins.HttpListener
{
    public interface IVerifyingRequestHandler
    {
        void AssertHeadersCaseInsensitivelyContain(string expectedKey, string expectedValue);
        void AssertHeaderCountIs(int expectedNumberOfHeaders);
        void AssertThatTheRequestBodyIsEmpty();
        void AssertThatThereAreNoQueryParameters();
        void AssertPathIs(string expectedPath);
        void AssertQueryParametersContain(string expectedKey, string expectedValue);
        void SetupRequestHandlerToRespondWith(Response response);
    }
}