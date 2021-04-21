using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace WebCallerClient.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("1")]
        public string GetHello1()
        {
            return "TeacherService.Hello1".WpcParam("Jim").Get();
        }


        [HttpGet("2")]
        public DateTime GetHello2()
        {
            return "TeacherService.Hello2".WpcParam(DateTime.Now).Get<DateTime>();
        }


        [HttpGet("3")]
        public int GetHello3()
        {
            return "TeacherService.Hello3".WpcParam(12.34).Get<int>();
        }


        [HttpGet("4")]
        public int GetHello4()
        {
            return "TeacherService.Hello4".WpcParam(new { Value=12.34,time= DateTime.Now }).Get<int>();
        }


        [HttpGet("5")]
        public string GetHello5()
        {
            return "TeacherService.Hello5".NoWpcParam().Get();
        }

        [HttpGet("6")]
        public string GetHello6()
        {
            return "TeacherService.Hello6".WpcParam(new { Indexs = new int[] { 1,2,3,4 }, name="abc" }).Get();
        }

        [HttpGet("7")]
        public string GetHello7()
        {
            return "Hello7".WpcParam(new { Indexs = new int[] { 1, 2, 3, 4 }, name = "abc" }).Get();
        }

        [HttpGet("8")]
        public HttpStatusCode GetHello8()
        {
            return "TeacherService.Hello8".NoWpcParam().Execute();
        }

        [HttpGet("9")]
        public object GetHello9()
        {
            return "TeacherService.Hello9".NoWpcParam().Get<HttpStatusCode>();
        }
        [HttpGet("10")]
        public string GetHello10()
        {
            return "TestPluginService.Get".WpcParam("Jim").Get();
        }
        [HttpGet("11")]
        public HttpStatusCode GetHello11()
        {
            return "TestPluginService.Show".NoWpcParam().Execute();
        }
        [HttpGet("12")]
        public int GetHello12()
        {
            return "TeacherService.Hello7".NoWpcParam().Get<int>();
        }
        [HttpGet("13")]
        public HttpStatusCode GetHello13()
        {
            return "TeacherService.Hello11".NoWpcParam().Execute();
        }
        [HttpGet("14")]
        public int GetHello14()
        {
            return "TeacherService.Hello10".WpcParam(new int[] { 1,2,3,4,5,6,7}).Get<int>();
        }
    }
}
