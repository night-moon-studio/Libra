using Libra.Model;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Libra.Extension.Utils
{
    /// <summary>
    /// 参数处理程序
    /// </summary>
    public class LibraParameterHandler
    {

        protected readonly LibraProtocal _callMode;
        public LibraParameterHandler() { }
        public LibraParameterHandler(string caller, byte[] parameters = null)
        {
            _callMode = new LibraProtocal() { Flag = caller, Parameters = parameters };
        }


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
                        if (!Execute<bool>(urls[index]))
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
                        if (!Execute<bool>(urls[indexs[index]]))
                        {
                            cts.SetResult(false);
                        }
                    });
                    cts.SetResult(true);

                });

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
                        var result = Execute(urls[index]);
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
                        var result = Execute(urls[indexs[index]]);
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
                Parallel.For(0, urls.Length, index => { result[index] = Execute<S>(urls[index]); });
                return result;

            }
            else
            {

                var urls = LibraMulticastHostManagement.GetUrls(key);
                var result = new S[urls.Length];
                Parallel.For(0, indexs.Length, index => { result[indexs[index]] = Execute<S>(urls[indexs[index]]); });
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
                Parallel.For(0, urls.Length, index => { result[index] = Execute(urls[index]); });
                return result;

            }
            else
            {

                var urls = LibraMulticastHostManagement.GetUrls(key);
                var result = new HttpStatusCode[urls.Length];
                Parallel.For(0, indexs.Length, index => { result[indexs[index]] = Execute(urls[indexs[index]]); });
                return result;

            }

        }


        /// <summary>
        /// 执行一组远程请求,并返回元祖数组结果
        /// </summary>
        /// <param name="key">组播KEY</param>
        /// <returns></returns>
        public (string Url,S Result)[] MulticastTupleResult<S>(string key, params int[] indexs)
        {

            if (indexs.Length == 0)
            {

                var urls = LibraMulticastHostManagement.GetUrls(key);
                (string Url, S Result)[] result = new (string Url, S Result)[urls.Length];
                Parallel.For(0, urls.Length, index => { result[index] =(urls[index].Authority, Execute<S>(urls[index])); });
                return result;

            }
            else
            {

                var urls = LibraMulticastHostManagement.GetUrls(key);
                (string Url, S Result)[] result = new (string Url, S Result)[indexs.Length];
                Parallel.For(0, indexs.Length, index => { result[indexs[index]] = (urls[index].Authority, Execute<S>(urls[indexs[index]])); });
                return result;

            }

        }


        /// <summary>
        /// 执行一组远程请求,并返回元祖数组结果
        /// </summary>
        /// <param name="key">组播KEY</param>
        /// <returns></returns>
        public (string Url, HttpStatusCode Result)[] MulticastTupleResult(string key, params int[] indexs)
        {

            if (indexs.Length == 0)
            {

                var urls = LibraMulticastHostManagement.GetUrls(key);
                (string Url, HttpStatusCode Result)[] result = new (string Url, HttpStatusCode Result)[urls.Length];
                Parallel.For(0, urls.Length, index => { result[index] = (urls[index].Authority, Execute(urls[index])); });
                return result;

            }
            else
            {

                var urls = LibraMulticastHostManagement.GetUrls(key);
                (string Url, HttpStatusCode Result)[] result = new (string Url, HttpStatusCode Result)[indexs.Length];
                Parallel.For(0, indexs.Length, index => { result[indexs[index]] = (urls[index].Authority, Execute(urls[indexs[index]])); });
                return result;

            }

        }


        /// <summary>
        /// 指定地址执行返回实体
        /// </summary>
        /// <typeparam name="S">返回值类型</typeparam>
        /// <param name="url">远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public S Execute<S>(string url)
        {
            return Execute<S>(new Uri(url));
        }


        /// <summary>
        /// 指定地址执行返回实体
        /// </summary>
        /// <typeparam name="S">返回值类型</typeparam>
        /// <param name="url">远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public S Execute<S>(Uri url)
        {

            var result = LibraRequestPool.Execute(url, _callMode);
            return LibraResultHandler<S>.GetResult(result);

        }


        /// <summary>
        /// 指定地址异步执行返回实体
        /// </summary>
        /// <typeparam name="S">返回值类型</typeparam>
        /// <param name="url">>远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public async Task<S> ExecuteAsync<S>(string url)
        {
            return Execute<S>(new Uri(url));
        }


        /// <summary>
        /// 指定地址异步执行返回实体
        /// </summary>
        /// <typeparam name="S">返回值类型</typeparam>
        /// <param name="url">>远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public async Task<S> ExecuteAsync<S>(Uri url)
        {
            return Execute<S>(url);

        }


        /// <summary>
        /// 指定远程地址, 执行 Void 方法
        /// </summary>
        /// <param name="url">请求地址(例如: http://xxxx/Libra )</param>
        /// <returns></returns>
        public HttpStatusCode Execute(string url)
        {
            return Execute(new Uri(url));
        }


        /// <summary>
        /// 指定远程地址, 执行 Void 方法
        /// </summary>
        /// <param name="url">请求地址(例如: http://xxxx/Libra )</param>
        /// <returns></returns>
        public HttpStatusCode Execute(Uri url)
        {
            return LibraRequestPool.ExecuteVoid(url, _callMode);
        }


        /// <summary>
        /// 指定远程地址, 异步执行 Void 方法
        /// </summary>
        /// <param name="url">请求地址(例如: http://xxxx/Libra )</param>
        /// <returns></returns>
        public async Task<HttpStatusCode> ExecuteAsync(string url)
        {
            return Execute(new Uri(url));
        }


        /// <summary>
        /// 指定远程地址, 异步执行 Void 方法
        /// </summary>
        /// <param name="url">请求地址(例如: http://xxxx/Libra )</param>
        /// <returns></returns>
        public async Task<HttpStatusCode> ExecuteAsync(Uri url)
        {
            return Execute(url);

        }
       

        /// <summary>
        /// 不指定地址, 使用 BaseUrl, 执行返回实体
        /// </summary>
        /// <typeparam name="S">返回值类型</typeparam>
        /// <returns></returns>
        public S Execute<S>()
        {
            var result = LibraRequestPool.Execute(_callMode);
            return LibraResultHandler<S>.GetResult(result);
        }


        /// <summary>
        /// 不指定地址, 使用 BaseUrl, 执行返回实体
        /// </summary>
        /// <typeparam name="S">返回值类型</typeparam>
        /// <returns></returns>
        public async Task<S> ExecuteAsync<S>()
        {
            return Execute<S>();
        }


        /// <summary>
        /// 不指定远程地址, 使用 BaseUrl, 执行 Void 方法
        /// </summary>
        /// <returns></returns>
        public HttpStatusCode Execute()
        {
            return LibraRequestPool.ExecuteVoid(_callMode);
        }
        // <summary>
        /// 不指定远程地址, 使用 BaseUrl, 执行 Void 方法
        /// </summary>
        /// <returns></returns>
        public async Task<HttpStatusCode> ExecuteAsync()
        {
            return Execute();
        }

    }

    /// <summary>
    /// 参数处理程序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LibraParameterHandler<T> : LibraParameterHandler
    {

        public LibraParameterHandler(string caller, T parameter) : base(caller, _serialize(parameter))
        {

        }


        private static readonly Func<T, byte[]> _serialize;
        static LibraParameterHandler()
        {
            if (typeof(T).IsPrimitive || typeof(T).IsValueType)
            {

                //基元类型及值类型使用 LibraSingleParameter 进行代理
                _serialize = (obj) => JsonSerializer.SerializeToUtf8Bytes(new LibraSingleParameter<T>() { Value = obj });

            }
            else if (typeof(T) == typeof(byte[]))
            {

                //byte数组直接返回
                LibraParameterHandler<byte[]>._serialize = item => item;

            }
            else
            {

                //其他复杂类型
                _serialize = (obj) =>
                {
                    if (obj == null)
                    {
                        return null;
                    }
                    return JsonSerializer.SerializeToUtf8Bytes(obj);
                };

            }


        }

    }
}
