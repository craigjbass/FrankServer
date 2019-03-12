using System.Text;

namespace Frank.DTO
{
    public struct Response
    {
        public int Status;
        public byte[] Body { get; set; }
    }
    
    public static class ResponseHelpers
    {
        public static Response BodyFromString(this Response response, string body)
        {
            response.Body = Encoding.UTF8.GetBytes(body);
            return response;
        }
    }
}