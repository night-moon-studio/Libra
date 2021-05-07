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
        public virtual S GetResult<S>()
        {
            var result = LibraClientPool.BytesResult(_route, _protocal);
            return LibraReadHandler<S>.GetResult(result);
        }
        public virtual async Task<S> GetResultAsync<S>()
        {
            var result = LibraClientPool.BytesResult(_route, _protocal);
            return LibraReadHandler<S>.GetResult(result);
        }

        /// <summary>
        /// 不指定地址, 使用 BaseUrl, 执行返回比特流
        /// </summary>
        /// <returns></returns>
        public virtual byte[] GetBytes()
        {
            return LibraClientPool.BytesResult(_route, _protocal);
        }
        public virtual async Task<byte[]> GetBytesAsync()
        {
            return LibraClientPool.BytesResult(_route, _protocal);
        }

        /// <summary>
        /// 不指定远程地址, 使用 BaseUrl, 执行 Void 方法
        /// </summary>
        /// <returns></returns>
        public virtual HttpStatusCode GetCode()
        {
            return LibraClientPool.CodeResult(_route, _protocal);
        }
        public virtual async Task<HttpStatusCode> GetCodeAsync()
        {
            return LibraClientPool.CodeResult(_route, _protocal);
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
        public override S GetResult<S>()
        {
            return GetResult<S>(_uri);
        }
        public override Task<S> GetResultAsync<S>()
        {
            return GetResultAsync<S>(_uri);
        }

        /// <summary>
        /// 不指定地址, 使用 BaseUrl, 执行返回比特流
        /// </summary>
        /// <returns></returns>
        public override byte[] GetBytes()
        {
            return GetBytes(_uri);
        }
        public override Task<byte[]> GetBytesAsync()
        {
            return GetBytesAsync(_uri);
        }

        /// <summary>
        /// 不指定远程地址, 使用 BaseUrl, 执行 Void 方法
        /// </summary>
        /// <returns></returns>
        public override HttpStatusCode GetCode()
        {
            return GetCode(_uri);
        }
        public override Task<HttpStatusCode> GetCodeAsync()
        {
            return GetCodeAsync(_uri);
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
