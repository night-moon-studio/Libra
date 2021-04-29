using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{

    /// <summary>
    /// Libra 中间件配置选项类
    /// </summary>
    public class LibraOption
    {

        /// <summary>
        /// 设置映射调用
        /// </summary>
        /// <param name="key">对外暴漏的key</param>
        /// <param name="caller">"类名.方法名"</param>
        /// <returns></returns>
        public LibraOption CallerMapper(string key, string caller)
        {
            LibraTypeManagement.AddMapper(key, caller);
            return this;
        }


        /// <summary>
        /// 允许被调用的类型
        /// </summary>
        /// <typeparam name="T">实现该接口的类允许被调用</typeparam>
        /// <param name="typs"></param>
        /// <returns></returns>
        public LibraOption AllowTypes<T>(params Type[] typs)
        {
            LibraTypeManagement.AddType<T>(typs);
            return this;
        }


        /// <summary>
        /// 允许被调用的类型
        /// </summary>
        /// <param name="typs"></param>
        /// <returns></returns>
        public LibraOption AllowTypes(params Type[] typs)
        {
            LibraTypeManagement.AddType(typs);
            return this;
        }


        /// <summary>
        /// 允许被调用的程序集
        /// </summary>
        /// <typeparam name="T">实现该接口的类允许被调用</typeparam>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public LibraOption AllowAssembly<T>(Assembly assembly)
        {
            var types = assembly.GetTypes();
            return AllowTypes<T>(types);
        }


        /// <summary>
        /// 允许被调用的程序集
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public LibraOption AllowAssembly(Assembly assembly)
        {
            var types = assembly.GetTypes();
            return AllowTypes(types);
        }
    }

}
