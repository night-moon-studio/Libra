using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebServiceProvider.Service
{
    public interface IStudent1
    {
        string GetName(string a);
        ValueTask<bool> GetBool(int a);
        Task<string> GetString(int a, string b);
    }
}
