using System.Text;

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
    }

    public static class ResponseBuilders
    {
        public static Response Ok()
        {
            return new Response
            {
                Status = 200
            };
        }

        public static Response Created()
        {
            return new Response
            {
                Status = 201
            };
        }
    }
}