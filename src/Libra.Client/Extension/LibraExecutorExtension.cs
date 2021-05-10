using Libra.Client.Utils;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Libra
{

    public static class LibraExecutorExtension
    {

        /// <summary>
        /// 指定地址执行返回实体
        /// </summary>
        /// <typeparam name="S">返回值类型</typeparam>
        /// <param name="url">远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public static async Task<S> GetResultAsync<S>(this LibraExecutor handler, string url)
        {
            return await handler.GetResultAsync<S>(new Uri(url)).ConfigureAwait(false);
        }


        /// <summary>
        /// 指定远程地址, 执行 Void 方法
        /// </summary>
        /// <param name="url">请求地址(例如: http://xxxx )</param>
        /// <returns></returns>
        public static async ValueTask<HttpStatusCode> GetCodeAsync(this LibraExecutor handler, string url)
        {
            return await handler.GetCodeAsync(new Uri(url)).ConfigureAwait(false);
        }

    }
}
