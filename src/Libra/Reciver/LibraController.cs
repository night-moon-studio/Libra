using Libra.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Libra
{
    [Route("[controller]")]
    [ApiController]
    public class LibraController : ControllerBase
    {
        [HttpPost]
        public async void Run(LibraProtocal model)
        {
            var result = await LibraProtocalAnalysis.CallAsync(model.Flag, model.Parameters, Response);
            await Response.Body.WriteAsync(result);
        }
    }
}
