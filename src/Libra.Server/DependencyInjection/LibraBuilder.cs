﻿using Libra;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Libra配置
    /// </summary>
    public class LibraBuilder
    {
        private readonly IConfiguration _configuration;

        public LibraBuilder(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        /// <summary>
        /// 添加路由过滤器
        /// </summary>
        /// <param name="filterFunc">拦截方法</param>
        /// <returns></returns>
        public LibraBuilder ConfigureFilter(Func<string,HttpRequest,HttpResponse, ValueTask<bool>> filterFunc)
        {
            LibraMiddleware._hasFilter = filterFunc != null;
            LibraMiddleware.Filter = filterFunc;
            return this;
        }


        /// <summary>
        /// 配置 Libra 使用的 JSON 序列化选项
        /// </summary>
        /// <param name="optAction"></param>
        /// <returns></returns>
        public LibraBuilder ConfigureJson(Action<JsonSerializerOptions> optAction)
        {
            var option = new JsonSerializerOptions();
            optAction?.Invoke(option);
            LibraProxyCreator.JsonOption = option;
            return this;
        }

        public LibraBuilder ConfigureLibra(Func<LibraOption, LibraOption> optAction)
        {
            optAction?.Invoke(new LibraOption());
            return this;
        }

    }
}
