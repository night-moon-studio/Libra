using Libra.Model;
using Microsoft.AspNetCore.Mvc;

namespace Libra
{
    [Route("[controller]")]
    [ApiController]
    public class LibraController : ControllerBase
    {
        [HttpPost]
        public string Run(LibraProtocal model)
        {
            return LibraProtocalAnalysis.Call(model.Flag, model.Parameters);
        }
    }
}
