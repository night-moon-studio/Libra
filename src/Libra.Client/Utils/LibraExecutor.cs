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
        private string _domain;
        private Uri _url;
        private int _retry;
        private CancellationToken _cancellationToken;
        private Action<HttpRequestMessage> _requestHandler;
        public LibraExecutor(string route, string domain, Func<Stream, Task> protocal = null)
        {
            
            _route = route;
            _domain = domain;
            _protocal = protocal == null ? (item => Task.CompletedTask) : protocal;

        }


        /// <summary>
        /// 指定请求域
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public LibraExecutor WithDomain(string domain)
        {
            _domain = domain;
            return this;
        }

      
        /// <summary>
        /// 配置取消令牌
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public LibraExecutor WithCancellationToken(in CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            return this;
        }


        /// <summary>
        /// 配置请求委托
        /// </summary>
        /// <param name="requestHandler"></param>
        /// <returns></returns>
        public LibraExecutor WithRequest(Action<HttpRequestMessage> requestHandler)
        {
            _requestHandler = requestHandler;
            return this;
        }


        /// <summary>
        /// 配置URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public LibraExecutor WithUrl(Uri url)
        {
            _url = url;
            return this;
        }
        public LibraExecutor WithUrl(string url)
        {
            _url = new Uri(url);
            return this;
        }

        /// <summary>
        /// 配置重试次数
        /// </summary>
        /// <param name="retry"></param>
        /// <returns></returns>
        public LibraExecutor WithRetry(int retry)
        {
            _retry = retry;
            return this;
        }


        /// <summary>
        /// 指定远程地址, 执行 Void 方法
        /// </summary>
        /// <param name="url">请求地址(例如: http://xxxx)</param>
        /// <returns></returns>
        public async ValueTask<HttpStatusCode> GetCodeAsync()
        {

            var request = LibraClientPool.GetRequestInternal();
            try
            {
                request.WithConfiguration(_url, _route, _domain, _protocal, _requestHandler, _cancellationToken, _retry);
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
        public async Task<S> GetResultAsync<S>()
        {

            var request = LibraClientPool.GetRequestInternal();
            try
            {
                request.WithConfiguration(_url, _route, _domain, _protocal, _requestHandler, _cancellationToken, _retry);
                return await request.GetResultAsync<S>().ConfigureAwait(false);
            }
            finally
            {
                LibraClientPool.Collect(request);
            }

        }

    }

}
