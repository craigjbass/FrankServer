using System.Text;
using Newtonsoft.Json;

namespace Frank.API.WebDevelopers.DTO
{
    public struct Response
    {
        public int Status;
        public byte[] Body { get; set; }
    }

    public static class ResponseModifiers
    {
        public static Response BodyFromString(this Response response, string body)
        {
            response.Body = Encoding.UTF8.GetBytes(body);
            return response;
        }


        public static Response WithBody(this Response response, object body)
        {
            return response.BodyFromString(JsonConvert.SerializeObject(body));
        }
    }

    public static class ResponseBuilders
    {
        public static Response Ok() => NewResponseWithStatus(200);
        public static Response Created() => NewResponseWithStatus(201);
        private static Response NewResponseWithStatus(int code) => new Response {Status = code};
    }
}