using System;
using System.IO;
using System.Net;
using System.Net.Http;
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
        public LibraExecutor(string route, Func<Stream, Task> protocal = null)
        {
            _route = route;
            _protocal = protocal == null ? (item => Task.CompletedTask) : protocal;

        }

        #region 带有外部 URL 的API
        /// <summary>
        /// 指定地址执行返回比特流
        /// </summary>
        /// <param name="url">远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public byte[] GetBytes(Uri url, Action<HttpRequestMessage> requestHandler = null)
        {

            return LibraClientPool.BytesResult(url, _route, _protocal, requestHandler);

        }
        public async Task<byte[]> GetBytesAsync(Uri url, Action<HttpRequestMessage> requestHandler = null)
        {
            return LibraClientPool.BytesResult(url, _route, _protocal, requestHandler);
        }


        /// <summary>
        /// 指定远程地址, 执行 Void 方法
        /// </summary>
        /// <param name="url">请求地址(例如: http://xxxx )</param>
        /// <returns></returns>
        public HttpStatusCode GetCode(Uri url, Action<HttpRequestMessage> requestHandler = null)
        {
            return LibraClientPool.CodeResult(url, _route, _protocal, requestHandler);
        }
        public async Task<HttpStatusCode> GetCodeAsync(Uri url, Action<HttpRequestMessage> requestHandler = null)
        {
            return LibraClientPool.CodeResult(url, _route, _protocal, requestHandler);
        }


        /// <summary>
        /// 指定地址执行返回实体
        /// </summary>
        /// <typeparam name="S">返回值类型</typeparam>
        /// <param name="url">远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public S GetResult<S>(Uri url, Action<HttpRequestMessage> requestHandler = null)
        {

            var result = LibraClientPool.BytesResult(url, _route, _protocal, requestHandler);
            return LibraReadHandler<S>.GetResult(result);

        }
        public async Task<S> GetResultAsync<S>(Uri url, Action<HttpRequestMessage> requestHandler = null)
        {
            var result = LibraClientPool.BytesResult(url, _route, _protocal, requestHandler);
            return LibraReadHandler<S>.GetResult(result);
        }
        #endregion


        #region 使用默认 URL 的API
        /// <summary>
        /// 不指定地址, 使用 BaseUrl, 执行返回实体
        /// </summary>
        /// <typeparam name="S">返回值类型</typeparam>
        /// <returns></returns>
        public virtual S GetResult<S>(Action<HttpRequestMessage> requestHandler = null)
        {
            var result = LibraClientPool.BytesResult(_route, _protocal, requestHandler);
            return LibraReadHandler<S>.GetResult(result);
        }
        public virtual async Task<S> GetResultAsync<S>(Action<HttpRequestMessage> requestHandler = null)
        {
            var result = LibraClientPool.BytesResult(_route, _protocal, requestHandler);
            return LibraReadHandler<S>.GetResult(result);
        }

        /// <summary>
        /// 不指定地址, 使用 BaseUrl, 执行返回比特流
        /// </summary>
        /// <returns></returns>
        public virtual byte[] GetBytes(Action<HttpRequestMessage> requestHandler = null)
        {
            return LibraClientPool.BytesResult(_route, _protocal, requestHandler);
        }
        public virtual async Task<byte[]> GetBytesAsync(Action<HttpRequestMessage> requestHandler = null)
        {
            return LibraClientPool.BytesResult(_route, _protocal, requestHandler);
        }

        /// <summary>
        /// 不指定远程地址, 使用 BaseUrl, 执行 Void 方法
        /// </summary>
        /// <returns></returns>
        public virtual HttpStatusCode GetCode(Action<HttpRequestMessage> requestHandler = null)
        {
            return LibraClientPool.CodeResult(_route, _protocal, requestHandler);
        }
        public virtual async Task<HttpStatusCode> GetCodeAsync(Action<HttpRequestMessage> requestHandler = null)
        {
            return LibraClientPool.CodeResult(_route, _protocal, requestHandler);
        }
        #endregion


    }


    /// <summary>
    /// 参数处理程序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LibraExecutorWithoutUrl<T> : LibraExecutor
    {

        public LibraExecutorWithoutUrl(string route, T parameter) : base(route, LibraWirteHandler<T>.Serialize(parameter))
        {

        }

    }

    /// <summary>
    /// 参数处理程序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// 
    public class LibraExecutorWithUrl : LibraExecutor
    {
        private readonly Uri _uri;
        public LibraExecutorWithUrl(string route, Uri uri, Func<Stream, Task> protocal = null) : base(route, protocal)
        {
            _uri = uri;

        }

        #region 使用API
        /// <summary>
        /// 不指定地址, 使用 BaseUrl, 执行返回实体
        /// </summary>
        /// <typeparam name="S">返回值类型</typeparam>
        /// <returns></returns>
        public override S GetResult<S>(Action<HttpRequestMessage> requestHandler = null)
        {
            return GetResult<S>(_uri, requestHandler);
        }
        public override Task<S> GetResultAsync<S>(Action<HttpRequestMessage> requestHandler = null)
        {
            return GetResultAsync<S>(_uri, requestHandler);
        }

        /// <summary>
        /// 不指定地址, 使用 BaseUrl, 执行返回比特流
        /// </summary>
        /// <returns></returns>
        public override byte[] GetBytes(Action<HttpRequestMessage> requestHandler = null)
        {
            return GetBytes(_uri, requestHandler);
        }
        public override Task<byte[]> GetBytesAsync(Action<HttpRequestMessage> requestHandler = null)
        {
            return GetBytesAsync(_uri, requestHandler);
        }

        /// <summary>
        /// 不指定远程地址, 使用 BaseUrl, 执行 Void 方法
        /// </summary>
        /// <returns></returns>
        public override HttpStatusCode GetCode(Action<HttpRequestMessage> requestHandler = null)
        {
            return GetCode(_uri, requestHandler);
        }
        public override Task<HttpStatusCode> GetCodeAsync(Action<HttpRequestMessage> requestHandler = null)
        {
            return GetCodeAsync(_uri, requestHandler);
        }
        #endregion

    }

    public class LibraExecutorWithUrl<T> : LibraExecutorWithUrl
    {

        private readonly Uri _uri;
        public LibraExecutorWithUrl(string route, Uri uri, T parameter) : base(route, uri, LibraWirteHandler<T>.Serialize(parameter))
        {
            _uri = uri;

        }

    }
}
