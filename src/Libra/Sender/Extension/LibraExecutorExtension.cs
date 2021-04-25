using Libra.Extension.Utils;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Libra
{
    public static class LibraAsyncExecutorExtension 
    {
        /// <summary>
        /// 指定地址执行返回实体
        /// </summary>
        /// <typeparam name="S">返回值类型</typeparam>
        /// <param name="url">远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public static async Task<S> GetResultAsync<S>(this LibraExecutor handler, string url)
        {
            return handler.GetResult<S>(new Uri(url));
        }


        /// <summary>
        /// 指定远程地址, 执行 Void 方法
        /// </summary>
        /// <param name="url">请求地址(例如: http://xxxx/Libra )</param>
        /// <returns></returns>
        public static async Task<HttpStatusCode> GetCodeAsync(this LibraExecutor handler, string url)
        {
            return handler.GetCode(new Uri(url));
        }


        /// <summary>
        /// 指定远程地址, 执行并返回比特流
        /// </summary>
        /// <param name="url">请求地址(例如: http://xxxx/Libra )</param>
        /// <returns></returns>
        public static async Task<byte[]> GetBytesAsync(this LibraExecutor handler, string url)
        {
            return handler.GetBytes(new Uri(url));
        }
    }

    public static class LibraExecutorExtension
    {

        /// <summary>
        /// 指定地址执行返回实体
        /// </summary>
        /// <typeparam name="S">返回值类型</typeparam>
        /// <param name="url">远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public static S GetResult<S>(this LibraExecutor handler, string url)
        {
            return handler.GetResult<S>(new Uri(url));
        }


        /// <summary>
        /// 指定远程地址, 执行 Void 方法
        /// </summary>
        /// <param name="url">请求地址(例如: http://xxxx/Libra )</param>
        /// <returns></returns>
        public static HttpStatusCode GetCode(this LibraExecutor handler, string url)
        {
            return handler.GetCode(new Uri(url));
        }


        /// <summary>
        /// 指定远程地址, 执行并返回比特流
        /// </summary>
        /// <param name="url">请求地址(例如: http://xxxx/Libra )</param>
        /// <returns></returns>
        public static byte[] GetBytes(this LibraExecutor handler, string url)
        {
            return handler.GetBytes(new Uri(url));
        }

    }
}
