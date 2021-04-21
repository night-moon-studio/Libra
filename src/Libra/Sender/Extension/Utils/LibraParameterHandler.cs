using Libra.Model;
using System;
using System.Net;
using System.Text.Json;

namespace Libra.Extension.Utils
{
    /// <summary>
    /// 参数处理程序
    /// </summary>
    public class LibraParameterHandler
    {
        protected readonly LibraProtocal _callMode;
        public LibraParameterHandler() { }
        public LibraParameterHandler(string caller, string parameters = null)
        {
            _callMode = new LibraProtocal() { Flag = caller, Parameters = parameters };
        }

        /// <summary>
        /// 指定地址执行直接返回字符串
        /// </summary>
        /// <param name="url">远程服务的地址</param>
        /// <returns></returns>
        public string Get(string url)
        {
            return LibraRequest.Execute(url, _callMode);
        }

        /// <summary>
        /// 指定地址执行返回实体
        /// </summary>
        /// <typeparam name="S">返回值类型</typeparam>
        /// <param name="url">远程服务的地址</param>
        /// <returns></returns>
        public S Get<S>(string url)
        {

            var result = LibraRequest.Execute(url, _callMode);
            return LibraResultHandler<S>.GetResult(result);

        }

        /// <summary>
        /// 指定远程地址, 执行 Void 方法
        /// </summary>
        /// <param name="url">远程服务的地址</param>
        /// <returns></returns>
        public HttpStatusCode Execute(string url)
        {
            return LibraRequest.ExecuteVoid(url, _callMode);
        }


        /// <summary>
        /// 使用BaseUrl地址作为远程调用地址, 直接返回字符串
        /// </summary>
        /// <returns></returns>
        public string Get()
        {
            return LibraRequest.Execute(_callMode);
        }

        /// <summary>
        /// 不指定地址, 使用 BaseUrl, 执行返回实体
        /// </summary>
        /// <typeparam name="S">返回值类型</typeparam>
        /// <returns></returns>
        public S Get<S>()
        {
            var result = LibraRequest.Execute(_callMode);
            return LibraResultHandler<S>.GetResult(result);
        }

        /// <summary>
        /// 不指定远程地址, 使用 BaseUrl, 执行 Void 方法
        /// </summary>
        /// <returns></returns>
        public HttpStatusCode Execute()
        {
            return LibraRequest.ExecuteVoid(_callMode);
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


        private static readonly Func<T, string> _serialize;
        static LibraParameterHandler()
        {
            if (typeof(T).IsPrimitive || typeof(T).IsValueType)
            {
                _serialize = (obj) => JsonSerializer.Serialize(new LibraSingleParameter<T>() { Value = obj });
            }
            else if(typeof(T) == typeof(string))
            {
                _serialize = obj => obj.ToString();
            }
            else 
            {
                _serialize = (obj) =>
                {
                    if (obj == null)
                    {
                        return "";
                    }
                    return JsonSerializer.Serialize(obj);
                };
            }

        }

    }
}
