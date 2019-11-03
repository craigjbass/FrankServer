using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Frank.API.WebDevelopers.DTO
{
    public struct Response
    {
        internal int Status;
        internal Dictionary<string, string> Headers;
        internal byte[] Body;
    }

    public static class ResponseModifiers
    {
        public static Response BodyFromString(this Response response, string body)
        {
            response.Body = Encoding.UTF8.GetBytes(body);
            return response;
        }
        
        public static Response WithHeader(this Response response, string xHeaderHere, string aValue)
        {
            response.Headers.Add(xHeaderHere, aValue);
            return response;
        }
        
        public static Response WithJsonBody(this Response response, object body)
        {
            return response.BodyFromString(JsonConvert.SerializeObject(body));
        }
    }

    public static class ResponseBuilders
    {
        public static Response Ok() => NewResponseWithStatus(200);
        public static Response Created() => NewResponseWithStatus(201);
        public static Response NewResponseWithStatus(int code) => new Response
        {
            Status = code,
            Headers = new Dictionary<string, string>()
        };
    }
}