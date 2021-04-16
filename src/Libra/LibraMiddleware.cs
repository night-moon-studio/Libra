using Libra;
using Libra.Reciver;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using System.Text.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LibraMiddleware
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

    public class LibraOption
    {
        public LibraOption SetBaseUrl(string url)
        {
            LibraRequest.SetBaseUrl(url);
            return this;
        }
        public LibraOption FlagMapper(string flag, string mapper)
        {
            LibraTypeManagement.AddMapper(flag, mapper);
            return this;
        }
        public LibraOption AllowTypes<T>(params Type[] typs)
        {
            LibraTypeManagement.AddType<T>(typs);
            return this;
        }

        public LibraOption AllowTypes(params Type[] typs)
        {
            LibraTypeManagement.AddType(typs);
            return this;
        }

        public LibraOption AllowAssembly<T>(Assembly assembly)
        {
            var types = assembly.GetTypes();
            return AllowTypes<T>(types);
        }

        public LibraOption AllowAssembly(Assembly assembly)
        {
            var types = assembly.GetTypes();
            return AllowTypes(types);
        }
    }

}
