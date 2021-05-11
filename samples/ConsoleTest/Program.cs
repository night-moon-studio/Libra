using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Get();
            Console.ReadKey();
        }

        static async Task Get()
        {
            LibraClientPool.SetGlobalBaseUrl("https://localhost:5001/");
            var temp = await "TeacherService.Hello5".WpcParam().GetResultAsync<string>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var handler = await "TeacherService.Hello5".WpcParam().GetResultAsync<string>();
            stopwatch.Stop();
            Console.WriteLine("执行 1 次:" + stopwatch.Elapsed);
            stopwatch.Restart();
            for (int i = 0; i < 100; i++)
            {
                await "TeacherService.Hello5".WpcParam().GetResultAsync<string>();
            }
            stopwatch.Stop();
            Console.WriteLine("执行 100 次:" + stopwatch.Elapsed);
        }


        public static async Task Run()
        {
            await Run2().ConfigureAwait(false);
        }

        public static async Task Run2()
        {
            await Run3().ConfigureAwait(false);
        }

        public static async Task Run3()
        {

        }
    }
}
