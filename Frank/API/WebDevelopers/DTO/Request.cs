using System.Collections.Generic;

namespace Frank.API.WebDevelopers.DTO
{
    public struct Request
    {
        public string Path;
        public string Body;
        public IReadOnlyDictionary<string, string> QueryParameters;
        public IReadOnlyDictionary<string, string> Headers;
        public string Method;
    }

    public static class RequestConstructors
    {
        public static Request Post() => ARequest("POST");

        public static Request Get() => ARequest("GET");

        private static Request ARequest(string method) =>
            new Request
            {
                Path = "/",
                Body = "",
                QueryParameters = new Dictionary<string, string>(),
                Headers = new Dictionary<string, string>(),
                Method = method
            };
    }

    public static class RequestModifiers
    {
        public static Request WithPath(this Request request, string path)
        {
            request.Path = path;
            return request;
        }
    }
}