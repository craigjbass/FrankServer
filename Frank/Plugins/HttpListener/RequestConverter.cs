using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Frank.API.WebDevelopers.DTO;

namespace Frank.Plugins.HttpListener
{
    internal class RequestConverter
    {
        private readonly HttpListenerRequest _request;
        public RequestConverter(HttpListenerRequest request) => _request = request;

        public Request Convert()
        {
            var headers = Headers();
            return new Request
            {
                Path = Path(),
                Body = Body(),
                QueryParameters = QueryParameters(),
                Headers = headers,
                Cookies = Cookies(headers),
                Authentication = Authentication(headers)
            };
        }

        private string Body() => new StreamReader(_request.InputStream).ReadToEnd();

        private Dictionary<string, string> Headers() =>
            new Dictionary<string, string>(
                _request.Headers.AllKeys.ToDictionary(k => k, k => _request.Headers[k]),
                StringComparer.OrdinalIgnoreCase
            );

        private string Path() =>
            _request.RawUrl.Substring(
                0,
                QuestionMarkLocation(_request) ?? _request.RawUrl.Length
            );

        private Dictionary<string, string> QueryParameters() =>
            _request.QueryString.AllKeys.ToDictionary(k => k, k => _request.QueryString[k]);

        private static Dictionary<string, string> Cookies(Dictionary<string, string> headers)
        {
            if (!headers.TryGetValue("Cookie", out var cookieHeader) || string.IsNullOrEmpty(cookieHeader))
                return new Dictionary<string, string>();

            return ParseCookieHeader(cookieHeader);
        }

        internal static Dictionary<string, string> ParseCookieHeader(string cookieHeader)
        {
            var cookies = new Dictionary<string, string>();
            foreach (var pair in cookieHeader.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                var trimmed = pair.Trim();
                var equalsIndex = trimmed.IndexOf('=');
                if (equalsIndex < 0) continue;
                var name = trimmed.Substring(0, equalsIndex).Trim();
                var value = trimmed.Substring(equalsIndex + 1).Trim();
                if (!string.IsNullOrEmpty(name))
                    cookies[name] = value;
            }
            return cookies;
        }

        private static Authentication Authentication(Dictionary<string, string> headers)
        {
            if (!headers.TryGetValue("Authorization", out var authHeader) || string.IsNullOrEmpty(authHeader))
                return new Authentication();

            return ParseAuthorizationHeader(authHeader);
        }

        internal static Authentication ParseAuthorizationHeader(string authHeader)
        {
            var spaceIndex = authHeader.IndexOf(' ');
            if (spaceIndex < 0)
                return new Authentication { Scheme = authHeader, Parameter = null };

            return new Authentication
            {
                Scheme = authHeader.Substring(0, spaceIndex),
                Parameter = authHeader.Substring(spaceIndex + 1)
            };
        }

        private static int? QuestionMarkLocation(HttpListenerRequest request)
        {
            var markLocation = request.RawUrl.IndexOf('?');
            if (markLocation < 0) return null;
            return markLocation;
        }
    }
}