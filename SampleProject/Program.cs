using System.Threading;
using static Frank.API.WebDevelopers.DTO.ResponseBuilders;

namespace SampleProject
{
    class Program
    {
        static void Main()
        {
            System.Frank
                .Configure()
                .WithRoutes(
                    router => { router.Get("/test", () => Ok()); }
                )
                .Build()
                .Start();


            SpinWait.SpinUntil(() => false);
        }
    }
}