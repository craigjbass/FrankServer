using System.Threading;

namespace SampleProject
{
    class Program
    {

        static void Main()
        {
            new SampleProjectApp().CreateApplication().StartListeningOn(8000);
                
            SpinWait.SpinUntil(() => false);
        }

    }
}