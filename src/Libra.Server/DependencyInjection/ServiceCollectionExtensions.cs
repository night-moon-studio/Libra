using Libra;

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
        public static LibraBuilder AddLibraWpc(this IServiceCollection services)
        {

            NatashaInitializer.InitializeAndPreheating();
            services.AddSingleton(typeof(LibraBuilder));
            LibraProxyCreator.DIService = services;
            LibraProxyCreator.Provider = services.BuildServiceProvider();
            return LibraProxyCreator.Provider.GetService<LibraBuilder>();

        }

    }

}
