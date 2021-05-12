using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace WebCallerClient.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("0")]
        public async Task<string> GetHello0()
        {
            return await "TeacherService.Hello".WpcParam<string>(null).GetResultAsync<string>().ConfigureAwait(false);
        }
        [HttpGet("1")]
        public async Task<string> GetHello1()
        {
            return await "TeacherService.Hello1".WpcParam("Jim").GetResultAsync<string>().ConfigureAwait(false);
        }


        [HttpGet("2")]
        public async Task<DateTime>  GetHello2()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var result =await "TeacherService.Hello2".WpcParam(DateTime.Now).GetResultAsync<DateTime>(item => { item.Headers.Add("a", "b"); }).ConfigureAwait(false);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);
            return result;

        }


        [HttpGet("3")]
        public async Task<int> GetHello3()
        {
            return await "TeacherService.Hello3".WpcParam(12.34).GetResultAsync<int>().ConfigureAwait(false);
        }


        [HttpGet("4")]
        public async Task<int> GetHello4()
        {
            return await "TeacherService.Hello4".WpcParam(new { Value=12.34,time= DateTime.Now }).GetResultAsync<int>().ConfigureAwait(false);
        }


        [HttpGet("5")]
        public async Task<string> GetHello5()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var handler = "TeacherService.Hello5".WpcParam();
            stopwatch.Stop();
            Console.WriteLine("准备:"+stopwatch.Elapsed);
            stopwatch.Reset();
            stopwatch.Start();
            var result = await handler.GetResultAsync<string>().ConfigureAwait(false);
            stopwatch.Stop();
            Console.WriteLine("执行:"+stopwatch.Elapsed);
            return result;
        }

        [HttpGet("6")]
        public async Task<string> GetHello6()
        {
            return await "TeacherService.Hello6".WpcParam(new { Indexs = new int[] { 1,2,3,4 }, name="abc" }).GetResultAsync<string>().ConfigureAwait(false);
        }

        [HttpGet("7")]
        public async Task<string> GetHello7()
        {
            return await "Hello7".WpcParam(new { Indexs = new int[] { 1, 2, 3, 4 }, name = "abc" }).GetResultAsync<string>().ConfigureAwait(false);
        }

        [HttpGet("8")]
        public async ValueTask<HttpStatusCode> GetHello8()
        {
            return await "TeacherService.Hello8".WpcParam().GetCodeAsync().ConfigureAwait(false);
        }

        [HttpGet("9")]
        public async Task<object> GetHello9()
        {
            return await "TeacherService.Hello9".WpcParam().GetResultAsync<byte[]>().ConfigureAwait(false);
        }
        [HttpGet("10")]
        public async Task<string> GetHello10()
        {
            return await "TestPluginService.Get".WpcParam("Jim").GetResultAsync<string>().ConfigureAwait(false);
        }
        [HttpGet("11")]
        public async ValueTask<HttpStatusCode> GetHello11()
        {
            return await "TestPluginService.Show".WpcParam().GetCodeAsync().ConfigureAwait(false);
        }
        [HttpGet("12")]
        public async ValueTask<int> GetHello12()
        {

            using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1020)))
            {
                return await "TeacherService.Hello7".WpcParam(cts.Token).GetResultAsync<int>().ConfigureAwait(false);
            }
            
        }
        [HttpGet("13")]
        public async ValueTask<HttpStatusCode> GetHello13()
        {
            return await "TeacherService.Hello11".WpcParam().GetCodeAsync().ConfigureAwait(false);
        }
        [HttpGet("14")]
        public async ValueTask<int> GetHello14()
        {
            return await "TeacherService.Hello10".WpcParam(new int[] { 1,2,3,4,5,6,7}).GetResultAsync<int>().ConfigureAwait(false);
        }
        [HttpGet("15")]
        public async Task<TestModel> GetHello15()
        {
            return await "TeacherService.GetNull".WpcParam().GetResultAsync<TestModel>().ConfigureAwait(false);
        }
        [HttpGet("16")]
        public async Task<TestModel> GetHello16()
        {
            return await "TeacherService.GetNotNull".WpcParam().GetResultAsync<TestModel>().ConfigureAwait(false);
        }

        [HttpGet("17")]
        public async Task<LibraMulticastResult<int>[]> GetHello17()
        {
            var result = await "TeacherService.Hello7".WpcParam().MulticastTupleResultAsync<int>("测试组");
            Debug.WriteLine(result);
            return result;
        }

        [HttpGet("18")]
        public async Task<LibraMulticastResult<string>[]> GetHello18()
        {
            var result = await "TeacherService.Hello5".WpcParam().MulticastTupleResultAsync<string>("测试组");
            Debug.WriteLine(result);
            return result;
        }

        [HttpGet("18.5")]
        public async Task<byte[]> GetHello185()
        {
            var result = await "TeacherService.GetBytes".WpcParam(new int[] { 1, 2, 3, 4, 5 }).GetResultAsync<byte[]>().ConfigureAwait(false);
            Debug.WriteLine(result);
            return result;
        }

        [HttpGet("18.6")]
        public async Task<object> GetHello186()
        {
            return "TeacherService.GetBytes".WpcParam<byte[]>(null).GetResultAsync<byte[]>().ConfigureAwait(false);
        }

        [HttpGet("19")]
        public async Task<object> GetHello19()
        {
            return "TeacherService.GetBytes".WpcParam(new int[] { 1,2,3,4,5}).MulticastArrayResultAsync<byte[]>("测试组");
        }


        [HttpGet("20")]
        public async Task<bool> GetHello20()
        {
            return await "TeacherService.Notify".WpcParam().MulticastNotifyAsync("测试组");
        }

        [HttpGet("21")]
        public async Task<bool> GetHello21()
        {
            return await "TeacherService.Notify1".WpcParam().MulticastNotifyAsync("测试组");
        }

        [HttpGet("22")]
        public async Task<string[]> GetHello22()
        {
            return await "TeacherService.GetStrings".WpcParam(new string[] {"a","b" }).GetResultAsync<string[]>();
        }

        [HttpGet("23")]
        public async Task<object> GetHello23()
        {
            return await "TeacherService.GetInts".WpcParam(new int[] {1, 2 }).GetResultAsync<int[]>();
        }
        [HttpGet("24")]
        public async Task<object> GetHello24()
        {
            return await "TeacherService.GetInts2".WpcParam(new byte[] { 1, 2 }).GetResultAsync<byte[]>().ConfigureAwait(false);
        }
        [HttpGet("25")]
        public async Task<string> GetHello25()
        {
            return await "IStudent.GetName".WpcParam().GetResultAsync<string>();
        }
        [HttpGet("26")]
        public async Task<TestModel> GetHello26()
        {
            return await "TeacherService.GetNotNull1".WpcParam().GetResultAsync<TestModel>().ConfigureAwait(false);
        }
        [HttpGet("27")]
        public async Task<string> GetHello27()
        {
            
            return await "TeacherService.GetStreamString".WpcParam("abc").GetResultAsync<string>().ConfigureAwait(false);
        }
    }
}
