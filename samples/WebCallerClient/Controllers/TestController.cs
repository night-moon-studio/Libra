using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace WebCallerClient.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("0")]
        public string GetHello0()
        {
            return "TeacherService.Hello".WpcParam<string>(null).GetResult<string>();
        }
        [HttpGet("1")]
        public string GetHello1()
        {
            return "TeacherService.Hello1".WpcParam("Jim").GetResult<string>();
        }


        [HttpGet("2")]
        public DateTime GetHello2()
        {
            return "TeacherService.Hello2".WpcParam(DateTime.Now).GetResult<DateTime>(item => { item.Headers.Add("a", "b"); });
        }


        [HttpGet("3")]
        public int GetHello3()
        {
            return "TeacherService.Hello3".WpcParam(12.34).GetResult<int>();
        }


        [HttpGet("4")]
        public int GetHello4()
        {
            return "TeacherService.Hello4".WpcParam(new { Value=12.34,time= DateTime.Now }).GetResult<int>();
        }


        [HttpGet("5")]
        public string GetHello5()
        {
            return "TeacherService.Hello5".NoWpcParam().GetResult<string>();
        }

        [HttpGet("6")]
        public string GetHello6()
        {
            return "TeacherService.Hello6".WpcParam(new { Indexs = new int[] { 1,2,3,4 }, name="abc" }).GetResult<string>();
        }

        [HttpGet("7")]
        public string GetHello7()
        {
            return "Hello7".WpcParam(new { Indexs = new int[] { 1, 2, 3, 4 }, name = "abc" }).GetResult<string>();
        }

        [HttpGet("8")]
        public HttpStatusCode GetHello8()
        {
            return "TeacherService.Hello8".NoWpcParam().GetCode();
        }

        [HttpGet("9")]
        public object GetHello9()
        {
            return "TeacherService.Hello9".NoWpcParam().GetBytes();
        }
        [HttpGet("10")]
        public string GetHello10()
        {
            return "TestPluginService.Get".WpcParam("Jim").GetResult<string>();
        }
        [HttpGet("11")]
        public HttpStatusCode GetHello11()
        {
            return "TestPluginService.Show".NoWpcParam().GetCode();
        }
        [HttpGet("12")]
        public int GetHello12()
        {
            return "TeacherService.Hello7".NoWpcParam().GetResult<int>();
        }
        [HttpGet("13")]
        public HttpStatusCode GetHello13()
        {
            return "TeacherService.Hello11".NoWpcParam().GetCode();
        }
        [HttpGet("14")]
        public int GetHello14()
        {
            return "TeacherService.Hello10".WpcParam(new int[] { 1,2,3,4,5,6,7}).GetResult<int>();
        }
        [HttpGet("15")]
        public TestModel GetHello15()
        {
            return "TeacherService.GetNull".NoWpcParam().GetResult<TestModel>();
        }
        [HttpGet("16")]
        public TestModel GetHello16()
        {
            return "TeacherService.GetNotNull".NoWpcParam().GetResult<TestModel>();
        }

        [HttpGet("17")]
        public LibraMulticastResult<int>[] GetHello17()
        {
            var result = "TeacherService.Hello7".NoWpcParam().MulticastTupleResult<int>("测试组");
            Debug.WriteLine(result);
            return result;
        }

        [HttpGet("18")]
        public LibraMulticastResult<string>[] GetHello18()
        {
            var result = "TeacherService.Hello5".NoWpcParam().MulticastTupleResult<string>("测试组");
            Debug.WriteLine(result);
            return result;
        }

        [HttpGet("18.5")]
        public object GetHello185()
        {
            var result = "TeacherService.GetBytes".WpcParam(new int[] { 1, 2, 3, 4, 5 }).GetBytes();
            Debug.WriteLine(result);
            return result;
        }

        [HttpGet("18.6")]
        public object GetHello186()
        {
            return "TeacherService.GetBytes".WpcParam<byte[]>(null).GetBytes();
        }

        [HttpGet("19")]
        public object GetHello19()
        {
            return "TeacherService.GetBytes".WpcParam(new int[] { 1,2,3,4,5}).MulticastArrayResult<byte[]>("测试组");
        }


        [HttpGet("20")]
        public async Task<bool> GetHello20()
        {
            return await "TeacherService.Notify".NoWpcParam().MulticastNotifyAsync("测试组");
        }

        [HttpGet("21")]
        public async Task<bool> GetHello21()
        {
            return await "TeacherService.Notify1".NoWpcParam().MulticastNotifyAsync("测试组");
        }

        [HttpGet("22")]
        public object GetHello22()
        {
            return "TeacherService.GetStrings".WpcParam(new string[] {"a","b" }).GetResult<string[]>();
        }

        [HttpGet("23")]
        public object GetHello23()
        {
            return "TeacherService.GetInts".WpcParam(new int[] {1, 2 }).GetResult<int[]>();
        }
        [HttpGet("24")]
        public object GetHello24()
        {
            return "TeacherService.GetInts2".WpcParam(new byte[] { 1, 2 }).GetBytes();
        }
        [HttpGet("25")]
        public string GetHello25()
        {
            return "IStudent.GetName".NoWpcParam().GetResult<string>();
        }
    }
}
