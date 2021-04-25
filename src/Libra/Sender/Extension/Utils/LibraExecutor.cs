using Libra.Extension.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Libra.Extension.Utils
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

        #region 组播通知
        /// <summary>
        /// 通知一组远程主机,并返回通知是否成功
        /// </summary>
        /// <param name="key">组播KEY</param>
        /// <returns></returns>
        public Task<bool> MulticastNotifyAsync<TBool>(string key, params int[] indexs)
        {

            TaskCompletionSource<bool> cts = new TaskCompletionSource<bool>();
            if (indexs.Length == 0)
            {

                Task.Run(() =>
                {

                    var urls = LibraMulticastHostManagement.GetUrls(key);
                    Parallel.For(0, urls.Length, index =>
                    {
                        if (!GetResult<bool>(urls[index]))
                        {
                            cts.SetResult(false);
                        }
                    });
                    cts.SetResult(true);

                });


            }
            else
            {
                Task.Run((Action)(() =>
                {

                    var urls = LibraMulticastHostManagement.GetUrls(key);
                    Parallel.For(0, urls.Length, index =>
                    {
                        if (!GetResult<bool>(urls[indexs[index]]))
                        {
                            cts.SetResult(false);
                        }
                    });
                    cts.SetResult(true);

                }));

            }
            return cts.Task;

        }


        /// <summary>
        /// 通知一组远程主机,并返回通知是否成功
        /// </summary>
        /// <param name="key">组播KEY</param>
        /// <returns></returns>
        public Task<bool> MulticastNotifyAsync(string key, params int[] indexs)
        {

            TaskCompletionSource<bool> cts = new TaskCompletionSource<bool>();
            if (indexs.Length == 0)
            {

                Task.Run(() =>
                {

                    var urls = LibraMulticastHostManagement.GetUrls(key);
                    Parallel.For(0, urls.Length, index =>
                    {
                        var result = GetCode(urls[index]);
                        if (result != HttpStatusCode.OK && result != HttpStatusCode.NoContent)
                        {
                            cts.SetResult(false);
                        }
                    });
                    cts.SetResult(true);

                });


            }
            else
            {
                Task.Run(() =>
                {

                    var urls = LibraMulticastHostManagement.GetUrls(key);
                    Parallel.For(0, indexs.Length, index =>
                    {
                        var result = GetCode(urls[indexs[index]]);
                        if (result != HttpStatusCode.OK && result != HttpStatusCode.NoContent)
                        {
                            cts.SetResult(false);
                        }
                    });
                    cts.SetResult(true);

                });

            }
            return cts.Task;

        }
        #endregion




        #region 带有外部 URL 的API
        /// <summary>
        /// 指定地址执行返回比特流
        /// </summary>
        /// <param name="url">远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public byte[] GetBytes(Uri url)
        {

            return LibraRequestPool.BytesResult(url, _route, _protocal);

        }
        public async Task<byte[]> GetBytesAsync(Uri url)
        {
            return LibraRequestPool.BytesResult(url, _route, _protocal);
        }


        /// <summary>
        /// 指定远程地址, 执行 Void 方法
        /// </summary>
        /// <param name="url">请求地址(例如: http://xxxx/Libra )</param>
        /// <returns></returns>
        public HttpStatusCode GetCode(Uri url)
        {
            return LibraRequestPool.CodeResult(url, _route, _protocal);
        }
        public async Task<HttpStatusCode> GetCodeAsync(Uri url)
        {
            return LibraRequestPool.CodeResult(url, _route, _protocal);
        }


        /// <summary>
        /// 指定地址执行返回实体
        /// </summary>
        /// <typeparam name="S">返回值类型</typeparam>
        /// <param name="url">远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public S GetResult<S>(Uri url)
        {

            var result = LibraRequestPool.BytesResult(url, _route, _protocal);
            return LibraReadHandler<S>.GetResult(result);

        }
        public async Task<S> GetResultAsync<S>(Uri url)
        {
            var result = LibraRequestPool.BytesResult(url, _route, _protocal);
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
            var result = LibraRequestPool.BytesResult(_route, _protocal);
            return LibraReadHandler<S>.GetResult(result);
        }
        public async Task<S> GetResultAsync<S>()
        {
            var result = LibraRequestPool.BytesResult(_route, _protocal);
            return LibraReadHandler<S>.GetResult(result);
        }

        /// <summary>
        /// 不指定地址, 使用 BaseUrl, 执行返回比特流
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return LibraRequestPool.BytesResult(_route, _protocal);
        }
        public async Task<byte[]> GetBytesAsync()
        {
            return LibraRequestPool.BytesResult(_route, _protocal);
        }

        /// <summary>
        /// 不指定远程地址, 使用 BaseUrl, 执行 Void 方法
        /// </summary>
        /// <returns></returns>
        public HttpStatusCode GetCode()
        {
            return LibraRequestPool.CodeResult(_route, _protocal);
        }
        public async Task<HttpStatusCode> GetCodeAsync()
        {
            return LibraRequestPool.CodeResult(_route, _protocal);
        }
        #endregion


        #region 组播返回值
        /// <summary>
        /// 执行一组远程请求,并返回数组结果
        /// </summary>
        /// <param name="key">组播KEY</param>
        /// <returns></returns>
        public S[] MulticastArrayResult<S>(string key, params int[] indexs)
        {

            if (indexs.Length == 0)
            {

                var urls = LibraMulticastHostManagement.GetUrls(key);
                var result = new S[urls.Length];
                Parallel.For(0, urls.Length, index => { result[index] = GetResult<S>(urls[index]); });
                return result;

            }
            else
            {

                var urls = LibraMulticastHostManagement.GetUrls(key);
                var result = new S[urls.Length];
                Parallel.For(0, indexs.Length, index => { result[indexs[index]] = GetResult<S>(urls[indexs[index]]); });
                return result;

            }

        }


        /// <summary>
        /// 执行一组远程请求,并返回数组结果
        /// </summary>
        /// <param name="key">组播KEY</param>
        /// <returns></returns>
        public HttpStatusCode[] MulticastArrayResult(string key, params int[] indexs)
        {

            if (indexs.Length == 0)
            {

                var urls = LibraMulticastHostManagement.GetUrls(key);
                var result = new HttpStatusCode[urls.Length];
                Parallel.For(0, urls.Length, index => { result[index] = GetCode(urls[index]); });
                return result;

            }
            else
            {

                var urls = LibraMulticastHostManagement.GetUrls(key);
                var result = new HttpStatusCode[urls.Length];
                Parallel.For(0, indexs.Length, index => { result[indexs[index]] = GetCode(urls[indexs[index]]); });
                return result;

            }

        }


        /// <summary>
        /// 执行一组远程请求,并返回元祖数组结果
        /// </summary>
        /// <param name="key">组播KEY</param>
        /// <returns></returns>
        public LibraMulticastResult<S>[] MulticastTupleResult<S>(string key, params int[] indexs)
        {

            if (indexs.Length == 0)
            {

                var urls = LibraMulticastHostManagement.GetUrls(key);
                var result = new LibraMulticastResult<S>[urls.Length];
                Parallel.For(0, urls.Length, index =>
                {
                    var url = urls[index];
                    result[index] = new LibraMulticastResult<S>(url.Authority, GetResult<S>(url));

                });
                return result;

            }
            else
            {

                var urls = LibraMulticastHostManagement.GetUrls(key);
                var result = new LibraMulticastResult<S>[indexs.Length];
                Parallel.For(0, indexs.Length, index =>
                {
                    var url = urls[indexs[index]];
                    result[indexs[index]] = new LibraMulticastResult<S>(url.Authority, GetResult<S>(url));
                });
                return result;
            }

        }



        /// <summary>
        /// 执行一组远程请求,并返回元祖数组结果
        /// </summary>
        /// <param name="key">组播KEY</param>
        /// <returns></returns>
        public LibraMulticastResult[] MulticastTupleResult(string key, params int[] indexs)
        {

            if (indexs.Length == 0)
            {

                var urls = LibraMulticastHostManagement.GetUrls(key);
                var result = new LibraMulticastResult[urls.Length];
                Parallel.For(0, urls.Length, index =>
                {
                    var url = urls[index];
                    result[index] = new LibraMulticastResult(url.Authority, GetCode(url));

                });
                return result;

            }
            else
            {

                var urls = LibraMulticastHostManagement.GetUrls(key);
                var result = new LibraMulticastResult[indexs.Length];
                Parallel.For(0, indexs.Length, index =>
                {
                    var url = urls[indexs[index]];
                    result[indexs[index]] = new LibraMulticastResult(url.Authority, GetCode(url));
                });
                return result;
            }

        }
        #endregion


    }
}
