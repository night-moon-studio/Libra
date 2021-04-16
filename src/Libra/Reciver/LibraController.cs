using Libra.Model;
using Microsoft.AspNetCore.Mvc;

namespace Libra
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraController : ControllerBase
    {
        [HttpPost]
        public string Run(LibraProtocalModel model)
        {
            return LibraProtocalAnalysis.Call(model.Flag, model.Parameters);
        }
    }
}
