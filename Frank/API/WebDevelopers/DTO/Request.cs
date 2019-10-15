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
}