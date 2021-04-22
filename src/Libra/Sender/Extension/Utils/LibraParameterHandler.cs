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

        /*
        /// <summary>
        /// 执行一组远程请求,并返回一组结果
        /// </summary>
        /// <param name="key">组播KEY</param>
        /// <returns></returns>
        public string[] MulticastGet(string key, params int[] indexs)
        {
            if (indexs.Length == 0)
            {

                var urls = LibraMulticastHostManagement.GetUrls(key);
                string[] result = new string[urls.Length];
                Parallel.For(0, urls.Length, index => { result[index] = Get(urls[index]); });
                return result;
            }
            else
            {

                var urls = LibraMulticastHostManagement.GetUrls(key);
                string[] result = new string[indexs.Length];
                Parallel.For(0, indexs.Length, index => { result[indexs[index]] = Get(urls[indexs[index]]); });
                return result;

            }
        }
        */

        /// <summary>
        /// 执行一组远程请求,并返回一组结果
        /// </summary>
        /// <param name="key">组播KEY</param>
        /// <returns></returns>
        public S[] MulticastGet<S>(string key, params int[] indexs)
        {

            if (indexs.Length == 0)
            {

                var urls = LibraMulticastHostManagement.GetUrls(key);
                S[] result = new S[urls.Length];
                Parallel.For(0, urls.Length, index => { result[index] = Get<S>(urls[index]); });
                return result;

            }
            else
            {

                var urls = LibraMulticastHostManagement.GetUrls(key);
                S[] result = new S[indexs.Length];
                Parallel.For(0, indexs.Length, index => { result[indexs[index]] = Get<S>(urls[indexs[index]]); });
                return result;

            }

        }


        /// <summary>
        /// 执行一组远程请求,并返回一组结果
        /// </summary>
        /// <param name="key">组播KEY</param>
        /// <returns></returns>
        public HttpStatusCode[] MulticastExecute(string key, params int[] indexs)
        {

            if (indexs.Length == 0)
            {

                var urls = LibraMulticastHostManagement.GetUrls(key);
                HttpStatusCode[] result = new HttpStatusCode[urls.Length];
                Parallel.For(0, urls.Length, index => { result[index] = Execute(urls[index]); });
                return result;

            }
            else
            {

                var urls = LibraMulticastHostManagement.GetUrls(key);
                HttpStatusCode[] result = new HttpStatusCode[indexs.Length];
                Parallel.For(0, indexs.Length, index => { result[indexs[index]] = Execute(urls[indexs[index]]); });
                return result;

            }

        }


        /*
        /// <summary>
        /// 指定地址执行直接返回字符串
        /// </summary>
        /// <param name="url">远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public string Get(string url)
        {
            return Get(new Uri(url));
        }
        /// <summary>
        /// 指定地址执行直接返回字符串
        /// </summary>
        /// <param name="url">远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public string Get(Uri url)
        {
            return LibraRequestPool.Execute(url, _callMode);
        }
        /// <summary>
        /// 指定地址异步执行,返回字符串
        /// </summary>
        /// <param name="url">远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public async Task<string> GetAsync(string url)
        {
            return Get(new Uri(url));
        }
        /// <summary>
        /// 指定地址异步执行,返回字符串
        /// </summary>
        /// <param name="url">远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public async Task<string> GetAsync(Uri url)
        {
            return Get(url);
        }
        */

        /// <summary>
        /// 指定地址执行返回实体
        /// </summary>
        /// <typeparam name="S">返回值类型</typeparam>
        /// <param name="url">远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public S Get<S>(string url)
        {
            return Get<S>(new Uri(url));
        }
        /// <summary>
        /// 指定地址执行返回实体
        /// </summary>
        /// <typeparam name="S">返回值类型</typeparam>
        /// <param name="url">远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public S Get<S>(Uri url)
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

        public async Task<S> GetAsync<S>(string url)
        {
            return Get<S>(new Uri(url));
        }
        /// <summary>
        /// 指定地址异步执行返回实体
        /// </summary>
        /// <typeparam name="S">返回值类型</typeparam>
        /// <param name="url">>远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public async Task<S> GetAsync<S>(Uri url)
        {
            return Get<S>(url);

        }
        /// <summary>
        /// 指定远程地址, 执行 Void 方法
        /// </summary>
        /// <param name="url">远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public HttpStatusCode Execute(string url)
        {
            return Execute(new Uri(url));
        }
        /// <summary>
        /// 指定远程地址, 执行 Void 方法
        /// </summary>
        /// <param name="url">远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public HttpStatusCode Execute(Uri url)
        {
            return LibraRequestPool.ExecuteVoid(url, _callMode);
        }
        /// <summary>
        /// 指定远程地址, 异步执行 Void 方法
        /// </summary>
        /// <param name="url">远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public async Task<HttpStatusCode> ExecuteAsync(string url)
        {
            return Execute(new Uri(url));
        }
        /// <summary>
        /// 指定远程地址, 异步执行 Void 方法
        /// </summary>
        /// <param name="url">远程服务的地址:应为 url + "/Libra"</param>
        /// <returns></returns>
        public async Task<HttpStatusCode> ExecuteAsync(Uri url)
        {
            return Execute(url);

        }
        /*
        /// <summary>
        /// 使用BaseUrl地址作为远程调用地址, 直接返回字符串
        /// </summary>
        /// <returns></returns>
        public string Get()
        {
            return LibraRequestPool.Execute(_callMode);
        }
        public async Task<string> GetAsync()
        {
            return Get();
        }*/

        /// <summary>
        /// 不指定地址, 使用 BaseUrl, 执行返回实体
        /// </summary>
        /// <typeparam name="S">返回值类型</typeparam>
        /// <returns></returns>
        public S Get<S>()
        {
            var result = LibraRequestPool.Execute(_callMode);
            return LibraResultHandler<S>.GetResult(result);
        }
        public async Task<S> GetAsync<S>()
        {
            return Get<S>();
        }
        /// <summary>
        /// 不指定远程地址, 使用 BaseUrl, 执行 Void 方法
        /// </summary>
        /// <returns></returns>
        public HttpStatusCode Execute()
        {
            return LibraRequestPool.ExecuteVoid(_callMode);
        }

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
                _serialize = (obj) => JsonSerializer.SerializeToUtf8Bytes(new LibraSingleParameter<T>() { Value = obj });
            }
            else
            {
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
