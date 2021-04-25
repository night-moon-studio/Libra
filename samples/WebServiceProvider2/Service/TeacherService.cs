using System;
using System.Threading.Tasks;

namespace WebServiceProvider2.Services
{
    public class TeacherService
    {

        public string Hello1(string studentName)
        {
            Console.WriteLine("1"+ studentName);
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


        public async Task<int> Hello7()
        {
            await Task.Delay(4000);
            return 2;
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



        public byte[] GetBytes(int[] arr)
        {
            return null;
        }

        public async void Notify()
        {
            await Task.Delay(4000);
            return;
        }
        public void Notify1()
        {
            return;
        }
    }
}
