using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebServiceProvider.Service
{
    public class Student : IStudent
    {
        public string GetName()
        {
            return "Jim";
        }
    }

    public class Student1 : IStudent1
    {
        public ValueTask<bool> GetBool(int a)
        {
            return new ValueTask<bool>(a == 1);
        }

        public string GetName(string a)
        {
            return a;
        }

        public Task<string> GetString(int a, string b)
        {
            return Task.FromResult(a+b);
        }
    }

}
