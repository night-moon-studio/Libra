using System;
using System.Threading.Tasks;

namespace WebServiceProvider.Services
{
    public class TeacherService
    {

        public string Hello1(string studentName)
        {
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
            return ((int)(value+3) * 100) + time.Hour;
        }

        public string Hello5()
        {
            return "aaaaaaa";
        }

        public void Hello8()
        {
            Console.WriteLine("haha8888");
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
            for (int i = 0; i < values.Length; i+=1)
            {
                result += values[i];
            }
            return result;
        }

        public TestModel GetNull()
        {
            return null;
        }

        public TestModel GetNotNull()
        {
            return new TestModel() { Indexs = new int[] { 1, 2, 3 }, Name = "a" };
        }

        public byte[] GetBytes(byte[] arr)
        {
            return arr;
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
}
