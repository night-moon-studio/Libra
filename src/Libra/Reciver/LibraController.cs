using Libra.Model;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Libra
{
    [Route("[controller]")]
    [ApiController]
    public class LibraController : ControllerBase
    {
        [HttpPost]
        public async Task<string> Run(LibraProtocal model)
        {
            return await LibraProtocalAnalysis.CallAsync(model.Flag, model.Parameters, Response);
        }
    }
}
