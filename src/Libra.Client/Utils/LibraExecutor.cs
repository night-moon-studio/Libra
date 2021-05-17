using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
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
        private readonly string _domain;
        private readonly CancellationToken _cancellationToken;
        public LibraExecutor(string route, string domain, in CancellationToken cancellationToken, Func<Stream, Task> protocal = null)
        {

            _route = route;
            _domain = domain;
            _protocal = protocal == null ? (item => Task.CompletedTask) : protocal;
            _cancellationToken = cancellationToken;

        }

      


        /// <summary>
        /// 指定远程地址, 执行 Void 方法
        /// </summary>
        /// <param name="url">请求地址(例如: http://xxxx )</param>
        /// <returns></returns>
        public async ValueTask<HttpStatusCode> GetCodeAsync(Action<HttpRequestMessage> requestHandler, Uri url = null)
        {
            return await GetCodeAsync(url, requestHandler).ConfigureAwait(false);
        }
        public async ValueTask<HttpStatusCode> GetCodeAsync(Uri url = null, Action<HttpRequestMessage> requestHandler = null)
        {

            var request = LibraClientPool.GetRequestInternal();
            try
            {
                request.ConfigClient(url, _route, _domain, _protocal, requestHandler, _cancellationToken);
                return await request.GetResponseCodeAsync().ConfigureAwait(false);
            }
            finally
            {
                LibraClientPool.Collect(request);
            }
            
        }


        /// <summary>
        /// 指定地址执行返回实体
        /// </summary>
        /// <typeparam name="S">返回值类型</typeparam>
        /// <param name="url">远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public async Task<S> GetResultAsync<S>(Action<HttpRequestMessage> requestHandler, Uri url = null)
        {
            return await GetResultAsync<S>(url, requestHandler).ConfigureAwait(false);
        }
        public async Task<S> GetResultAsync<S>(Uri url = null, Action<HttpRequestMessage> requestHandler = null)
        {

            var request = LibraClientPool.GetRequestInternal();
            try
            {
                request.ConfigClient(url, _route, _domain, _protocal, requestHandler, _cancellationToken);
                var content = await request.GetHttpContentAsync().ConfigureAwait(false);
                return await LibraReadHandler<S>.GetResult(content);
            }
            finally
            {
                LibraClientPool.Collect(request);
            }

        }

    }

}
