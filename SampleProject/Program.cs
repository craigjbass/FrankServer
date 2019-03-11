using System.Threading;
using Frank;

namespace SampleProject
{
    class Program
    {
        static void Main(string[] args)
        {

            var builder = new WebApplicationBuilder();
            builder.WithRoutes(router =>
            {
                router.Get("/test", () => 200);
            });
            var webApplication = builder.Build();
            webApplication.Start();
            
            
            SpinWait.SpinUntil(() => false);
        }
    }
}