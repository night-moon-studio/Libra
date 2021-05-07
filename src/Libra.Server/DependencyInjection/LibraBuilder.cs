﻿using Libra;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;

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