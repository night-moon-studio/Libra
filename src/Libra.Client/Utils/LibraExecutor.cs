using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Libra.Client.Utils
{

    /// <summary>
    /// Libra执行器
    /// </summary>
    public class LibraExecutor
    {
        private readonly Func<Stream, Task> _protocal;
        private readonly string _route;
        public LibraExecutor() { }
        public LibraExecutor(string route, Func<Stream, Task> protocal = null)
        {
            _route = route;
            if (protocal == null)
            {
                protocal = item => Task.CompletedTask;
            }
            _protocal = protocal;
        }

      

        #region 带有外部 URL 的API
        /// <summary>
        /// 指定地址执行返回比特流
        /// </summary>
        /// <param name="url">远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public byte[] GetBytes(Uri url)
        {

            return LibraClientPool.BytesResult(url, _route, _protocal);

        }
        public async Task<byte[]> GetBytesAsync(Uri url)
        {
            return LibraClientPool.BytesResult(url, _route, _protocal);
        }


        /// <summary>
        /// 指定远程地址, 执行 Void 方法
        /// </summary>
        /// <param name="url">请求地址(例如: http://xxxx )</param>
        /// <returns></returns>
        public HttpStatusCode GetCode(Uri url)
        {
            return LibraClientPool.CodeResult(url, _route, _protocal);
        }
        public async Task<HttpStatusCode> GetCodeAsync(Uri url)
        {
            return LibraClientPool.CodeResult(url, _route, _protocal);
        }


        /// <summary>
        /// 指定地址执行返回实体
        /// </summary>
        /// <typeparam name="S">返回值类型</typeparam>
        /// <param name="url">远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public S GetResult<S>(Uri url)
        {

            var result = LibraClientPool.BytesResult(url, _route, _protocal);
            return LibraReadHandler<S>.GetResult(result);

        }
        public async Task<S> GetResultAsync<S>(Uri url)
        {
            var result = LibraClientPool.BytesResult(url, _route, _protocal);
            return LibraReadHandler<S>.GetResult(result);
        }
        #endregion


        #region 使用默认 URL 的API
        /// <summary>
        /// 不指定地址, 使用 BaseUrl, 执行返回实体
        /// </summary>
        /// <typeparam name="S">返回值类型</typeparam>
        /// <returns></returns>
        public S GetResult<S>()
        {
            var result = LibraClientPool.BytesResult(_route, _protocal);
            return LibraReadHandler<S>.GetResult(result);
        }
        public async Task<S> GetResultAsync<S>()
        {
            var result = LibraClientPool.BytesResult(_route, _protocal);
            return LibraReadHandler<S>.GetResult(result);
        }

        /// <summary>
        /// 不指定地址, 使用 BaseUrl, 执行返回比特流
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return LibraClientPool.BytesResult(_route, _protocal);
        }
        public async Task<byte[]> GetBytesAsync()
        {
            return LibraClientPool.BytesResult(_route, _protocal);
        }

        /// <summary>
        /// 不指定远程地址, 使用 BaseUrl, 执行 Void 方法
        /// </summary>
        /// <returns></returns>
        public HttpStatusCode GetCode()
        {
            return LibraClientPool.CodeResult(_route, _protocal);
        }
        public async Task<HttpStatusCode> GetCodeAsync()
        {
            return LibraClientPool.CodeResult(_route, _protocal);
        }
        #endregion


    
    }
}
