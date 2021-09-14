using Libra;
using System;

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
        public static IServiceCollection AddLibraWpc(this IServiceCollection services,Func<LibraBuilder,LibraBuilder> func = default)
        {

            NatashaInitializer.InitializeAndPreheating();
            func?.Invoke(new LibraBuilder());
            return services;

        }

    }

}
