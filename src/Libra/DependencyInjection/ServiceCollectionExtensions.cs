using Libra;
using System;
using System.Text.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLibraWpc(this IServiceCollection services, Func<LibraOption, LibraOption> optAction)
        {
            NatashaInitializer.InitializeAndPreheating();
            LibraProtocalAnalysis.Provider = services.BuildServiceProvider();
            optAction?.Invoke(new LibraOption());
            return services;
        }

        public static IServiceCollection AddLibraJson(this IServiceCollection services, Action<JsonSerializerOptions> action)
        {
            action?.Invoke(LibraProtocalAnalysis.JsonOption);
            return services;
        }


    }

}
