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

        public Request Convert() => new Request
        {
            Path = Path(),
            Body = Body(),
            QueryParameters = QueryParameters(),
            Headers = Headers()
        };

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

        private static int? QuestionMarkLocation(HttpListenerRequest request)
        {
            var markLocation = request.RawUrl.IndexOf('?');
            if (markLocation < 0) return null;
            return markLocation;
        }
    }
}