using System;

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
    }
}
