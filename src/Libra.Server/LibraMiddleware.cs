using Libra;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Natasha.CSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public static class LibraMiddleware
    {
        private static readonly ConcurrentDictionary<string, ExecuteLibraMethod> _invokerMapping;
        private static DynamicDictionaryBase<string, ExecuteLibraMethod> _invokeFastCache;
        private static readonly ConcurrentDictionary<string, Func<string, string, HttpRequest, HttpResponse, ValueTask<bool>>> _filterMapping;
        private static DynamicDictionaryBase<string, Func<string, string, HttpRequest, HttpResponse, ValueTask<bool>>> _filterCache;
        static LibraMiddleware()
        {
            _invokerMapping = new ConcurrentDictionary<string, ExecuteLibraMethod>();
            _invokeFastCache = _invokerMapping.PrecisioTree();
            _filterMapping = new ConcurrentDictionary<string, Func<string, string, HttpRequest, HttpResponse, ValueTask<bool>>>();
            _filterCache = _filterMapping.PrecisioTree();
        }

        public static void AddFilter(string domain, Func<string, string, HttpRequest, HttpResponse, ValueTask<bool>> filter)
        {
            _filterMapping[domain] = filter;
            _filterCache = _filterMapping.PrecisioTree();
        }

        /// <summary>
        /// 批量移除已缓存的方法映射
        /// </summary>
        /// <param name="keys"></param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Remove(string domain, IEnumerable<string> keys)
        {

            ExecuteLibraMethod func = null;
            foreach (var item in keys)
            {
                var caller = $"{domain}:{item}";
                if (_invokerMapping.ContainsKey(caller))
                {
                    while (!_invokerMapping.TryRemove(caller, out func)) ;
                }
            }
            func?.DisposeDomain();
            _invokeFastCache = _invokerMapping.PrecisioTree();

        }

        /// <summary>
        /// 使用 Libra 远程调用服务
        /// </summary>
        /// <param name="app"></param>
        public static void UseLibraService(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {

                var request = context.Request;
                var response = context.Response;
                if (request.Headers.TryGetValue(LibraDefined.ROUTE, out var route))
                {
                    var domain = request.Headers[LibraDefined.DOMAIN];
                    if (_filterCache.TryGetValue(domain, out var filter))
                    {
                        if (!await filter(route, domain, request, response))
                        {
                            await response.CompleteAsync();
                            return;
                        }
                    }
                    var caller = $"{domain}:{route}";
                    if (!_invokeFastCache.TryGetValue(caller, out var func))
                    {

                        var (newFunc, message, code) = await LibraProxyCreator.CreateDelegate(route, domain, response);
                        if (newFunc == null)
                        {

                            response.StatusCode = code;
                            await response.WriteAsync(message).ConfigureAwait(false);
                            await response.CompleteAsync();
                            return;
                        }
                        else
                        {
                            //添加到字典
                            func = newFunc;
                            _invokerMapping[caller] = newFunc;
                            //从字典转换到精确快速查找树
                            _invokeFastCache = _invokerMapping.PrecisioTree();
                        }
                    }

                    await func(request, response);
                    await response.CompleteAsync();

                }
                else
                {
                    await next();
                }

            });

        }
    }
}
