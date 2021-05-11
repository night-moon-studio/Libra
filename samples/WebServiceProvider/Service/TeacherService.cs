using Libra.Model;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceProvider.Services
{
    public class TeacherService
    {
        public string Hello(string studentName)
        {
            return null;
        }
        public string Hello1(string studentName)
        {
            Console.WriteLine("1" + studentName);
            return "Hello" + studentName;
        }

        public DateTime Hello2(DateTime time)
        {
            return DateTime.Now;
        }

        public int Hello3(double value)
        {
            return (int)value * 10;
        }

        public int Hello4(double value, DateTime time)
        {
            return ((int)(value + 3) * 100) + time.Hour;
        }

        public string Hello5()
        {
            return "aaaaaaa";
        }

        public void Hello8()
        {
            Console.WriteLine("haha8888");
        }

        public Stream GetStreamString(string a)
        {
            MemoryStream memory = new MemoryStream(Encoding.UTF8.GetBytes(a));
            return memory;
        }

        public string Hello6(TestModel model)
        {
            var i = 0;
            foreach (var item in model.Indexs)
            {
                i += item;
            }
            return model.Name + i;
        }

        public async Task<int> Hello7()
        {
            await Task.Delay(1000);
            return 1;
        }

        public async Task Hello11()
        {
            await Task.Delay(1000);
            Console.WriteLine(2);
        }

        public int Hello10(int[] values)
        {
            int result = 0;
            for (int i = 0; i < values.Length; i += 1)
            {
                result += values[i];
            }
            return result;
        }

        public TestModel GetNull()
        {
            return null;
        }

        public TestModel GetNotNull(TestModel model)
        {
            return new TestModel() { Indexs = new int[] { 1, 2, 3 }, Name = "a" };
        }
        protected TestModel GetNotNull1(TestModel model)
        {
            return new TestModel() { Indexs = new int[] { 1, 2, 3 }, Name = "a" };
        }
        public byte[] GetBytes(int[] arr)
        {
            //Console.WriteLine(arr[0]);
            return null;
        }

        public string[] GetStrings(string[] arr)
        {
            //Console.WriteLine(arr[0]);
            return null;
        }
        public int[] GetInts(int[] arr)
        {
            //Console.WriteLine(arr[0]);
            return null;
        }
        public byte[] GetInts2(byte[] arr)
        {
            //Console.WriteLine(arr[0]);
            return null;
        }

        public async void Notify()
        {
            throw new Exception("");
            return;
        }

        public void Notify1()
        {
            return;
        }
    }

    //public static class Na41151da884a4636b704706c2ccbd793
    //{
    //    public static async System.Threading.Tasks.Task Invoke(Microsoft.AspNetCore.Http.HttpRequest request, Microsoft.AspNetCore.Http.HttpResponse response)
    //    {
    //        var parameters = Libra.LibraProxyCreator.Deserialize<LibraSingleParameter<System.String>>(request);
    //        var result = new LibraResult<System.String>() { Value = (new WebServiceProvider.Services.TeacherService()).Hello(parameters.Value) };
    //        await System.Text.Json.JsonSerializer.SerializeAsync(response.Body, result);
    //    }
    //}
}
