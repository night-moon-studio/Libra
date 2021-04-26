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
}
