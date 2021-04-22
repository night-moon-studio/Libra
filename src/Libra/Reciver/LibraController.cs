using Libra.Model;
using Microsoft.AspNetCore.Mvc;

namespace Libra
{
    [Route("[controller]")]
    [ApiController]
    public class LibraController : ControllerBase
    {

        /// <summary>
        /// 接受协议并执行方法返回结果
        /// </summary>
        /// <param name="protocal">接受到的协议内容</param>
        [HttpPost]
        public async void Run(LibraProtocal protocal)
        {
            var result = await LibraProtocalAnalysis.CallAsync(protocal.Flag, protocal.Parameters, Response).ConfigureAwait(false);
            await Response.Body.WriteAsync(result);
        }
    }
}
