using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServiceProvider.Services;

namespace WebServiceProvider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public async void Test()
        {
            //await Na41151da884a4636b704706c2ccbd793.Invoke(Request, Response);
        }
    }
}
