using Libra;
using System;
using System.Text.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 中间件扩展
    /// </summary>
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// 使用 LibraWpc 并初始化信息
        /// </summary>
        /// <param name="services"></param>
        /// <param name="optAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddLibraWpc(this IServiceCollection services, Func<LibraOption, LibraOption> optAction)
        {
            NatashaInitializer.InitializeAndPreheating();
            LibraCaller.Provider = services.BuildServiceProvider();
            optAction?.Invoke(new LibraOption());
            return services;
        }

        /// <summary>
        /// 配置 LibraWpc 接收端的 Json 选项
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IServiceCollection AddLibraJson(this IServiceCollection services, Action<JsonSerializerOptions> action)
        {
            action?.Invoke(LibraCaller.JsonOption);
            return services;
        }

    }

}
