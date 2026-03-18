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
        public IReadOnlyDictionary<string, string> Cookies;
        public Authentication Authentication;
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
                Method = method,
                Cookies = new Dictionary<string, string>(),
                Authentication = new Authentication()
            };
    }

    public static class RequestModifiers
    {
        public static Request WithPath(this Request request, string path)
        {
            request.Path = path;
            return request;
        }

        public static Request WithCookies(this Request request, IReadOnlyDictionary<string, string> cookies)
        {
            request.Cookies = cookies;
            return request;
        }

        public static Request WithAuthentication(this Request request, Authentication authentication)
        {
            request.Authentication = authentication;
            return request;
        }
    }
}