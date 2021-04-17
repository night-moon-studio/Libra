using Libra.Reciver;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
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
