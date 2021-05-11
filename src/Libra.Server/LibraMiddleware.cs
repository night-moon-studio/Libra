﻿using Libra;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Natasha.CSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.AspNetCore.Builder
{
    public static class LibraMiddleware
    {
        private static readonly ConcurrentDictionary<string, ExecuteLibraMethod> _invokerMapping;
        private static DynamicDictionaryBase<string, ExecuteLibraMethod> _invokeFastCache;

        static LibraMiddleware()
        {
            _invokerMapping = new ConcurrentDictionary<string, ExecuteLibraMethod>();
            _invokeFastCache = _invokerMapping.PrecisioTree();
        }


        /// <summary>
        /// 批量移除已缓存的方法映射
        /// </summary>
        /// <param name="keys"></param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Remove(IEnumerable<string> keys)
        {

            ExecuteLibraMethod func = null;
            foreach (var item in keys)
            {
                if (_invokerMapping.ContainsKey(item))
                {
                    while (!_invokerMapping.TryRemove(item, out func)) ;
                }
            }
            func?.DisposeDomain();
            _invokeFastCache = _invokerMapping.PrecisioTree();

        }

        internal static Func<string, HttpRequest, HttpResponse, bool> Filter = (route,request,response) => true;
        /// <summary>
        /// 使用 Libra 远程调用服务
        /// </summary>
        /// <param name="app"></param>
        public static async void UseLibraService(this IApplicationBuilder app)
        {
            
            app.Use(async (context,next) => {

                var request = context.Request;
                var response = context.Response;
                if (request.Headers.TryGetValue("Libra", out var route))
                {
                    if (!Filter(route,request,response))
                    {
                        return;
                    }
                    if (!_invokeFastCache.TryGetValue(route, out var func))
                    {
                        func = await LibraProxyCreator.CreateDelegate(route, response);
                        if (func==null)
                        {

                            return;

                        }
                        //添加到字典
                        _invokerMapping[route] = func;
                        //从字典转换到精确快速查找树
                        _invokeFastCache = _invokerMapping.PrecisioTree();
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
