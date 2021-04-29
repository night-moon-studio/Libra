using Microsoft.AspNetCore.Builder;
using Natasha.Framework;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;


/// <summary>
/// Libra 插件管理类
/// </summary>
public static class LibraPluginManagement
{
    private static readonly object _pluginLock = new object();
    private static readonly ConcurrentDictionary<string, ConcurrentQueue<string>> _pluginKeyCache;
    private static readonly ConcurrentDictionary<string, ConcurrentQueue<string>> _pluginTypesCache;
    private static readonly ConcurrentDictionary<string, DomainBase> _nameDomainCache;
    private static readonly ConcurrentDictionary<DomainBase, string> _domainPluginCache;
    static LibraPluginManagement()
    {
        _domainPluginCache = new ConcurrentDictionary<DomainBase, string>();
        _pluginTypesCache = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
        _pluginKeyCache = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
        _nameDomainCache = new ConcurrentDictionary<string, DomainBase>();
    }


    /// <summary>
    /// 添加插件 允许用接口类型进行约束
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    public static Assembly LoadPlugin(string path, params Type[] interfaces)
    {
        if (!_pluginKeyCache.ContainsKey(path))
        {
            lock (_pluginLock)
            {
                if (!_pluginKeyCache.ContainsKey(path))
                {
                    var domain = DomainManagement.Random;
                    var assembly = domain.LoadPluginFromStream(path);
                    var types = assembly.GetTypes();

                    _domainPluginCache[domain] = path;
                    _pluginKeyCache[path] = new ConcurrentQueue<string>();
                    _pluginTypesCache[path] = new ConcurrentQueue<string>();
                    foreach (var item in types)
                    {
                        if (interfaces.Length == 0)
                        {
                            var typeName = Reverser(item);
                            _nameDomainCache[typeName] = domain;
                            _pluginTypesCache[path].Enqueue(typeName);
                        }
                        else
                        {
                            for (int i = 0; i < interfaces.Length; i += 1)
                            {
                                if (item.IsImplementFrom(interfaces[i]))
                                {
                                    var typeName = Reverser(item);
                                    _nameDomainCache[typeName] = domain;
                                    _pluginTypesCache[path].Enqueue(typeName);
                                    break;
                                }
                            }
                        }
                    }
                    return assembly;
                }
            }
        }
        return null;
    }


    /// <summary>
    /// 添加域到调用KEY的映射
    /// </summary>
    /// <param name="domain"></param>
    /// <param name="key"></param>
    public static void AddRecoder(DomainBase domain, string key)
    {
        _pluginKeyCache[_domainPluginCache[domain]].Enqueue(key);
    }


    /// <summary>
    /// 通过类型名获取域
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static DomainBase GetPluginDominByType(string type)
    {
        if (_nameDomainCache.TryGetValue(type, out var domain))
        {
            return domain;
        }
        return null;
    }


    /// <summary>
    /// 清除所有引用,以便卸载
    /// </summary>
    /// <param name="pluginPath"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string Clear(string pluginPath)
    {

        lock (_pluginLock)
        {
            if (_pluginKeyCache.TryGetValue(pluginPath, out var queue))
            {
                LibraMiddleware.Remove(queue.ToArray());
            }

            DomainBase domain = default;
            foreach (var item in _pluginTypesCache[pluginPath])
            {
                while (!_nameDomainCache.TryRemove(item, out domain)) { }
            }
            var domainName = domain.Name;
            while (!_pluginKeyCache.TryRemove(pluginPath, out var temp))
            {

            }
            while (!_pluginTypesCache.TryRemove(pluginPath, out var temp))
            {

            }
            while (!_domainPluginCache.TryRemove(domain, out var temp))
            {

            }
            ((NatashaAssemblyDomain)domain).Dispose();
            return domainName;
        }
    }


    /// <summary>
    /// 卸载插件
    /// </summary>
    /// <param name="pluginPath"></param>
    public static bool UnloadPlugin(string pluginPath)
    {

        if (_pluginKeyCache.ContainsKey(pluginPath))
        {
            var name = Clear(pluginPath);
            for (int i = 0; i < 10; i += 1)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return DomainManagement.IsDeleted(name);
        }
        return true;
    }


    /// <summary>
    /// 类名反解
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns></returns>
    internal static string Reverser(Type type)
    {

        string fatherString = default;
        //外部类处理
        if (type.DeclaringType != null && type.FullName != null)
        {
            fatherString = Reverser(type.DeclaringType) + ".";
        }


        //后缀
        StringBuilder Suffix = new StringBuilder();


        //数组判别
        while (type.HasElementType)
        {

            if (type.IsArray)
            {

                int count = type.GetArrayRank();

                Suffix.Append("[");
                for (int i = 0; i < count - 1; i++)
                {
                    Suffix.Append(",");
                }
                Suffix.Append("]");

            }
            type = type.GetElementType();

        }


        //泛型判别
        if (type.IsGenericType)
        {

            StringBuilder result = new StringBuilder();
            result.Append($"{type.Name.Split('`')[0]}<");

            if (type.GenericTypeArguments.Length > 0)
            {

                result.Append(Reverser(type.GenericTypeArguments[0]));
                for (int i = 1; i < type.GenericTypeArguments.Length; i++)
                {

                    result.Append(',');
                    result.Append(Reverser(type.GenericTypeArguments[i]));

                }

            }

            result.Append('>');
            result.Append(Suffix);
            return fatherString + result.ToString();

        }
        else
        {

            //特殊类型判别
            if (type == typeof(void))
            {

                return "void";

            }
            if (fatherString == default && type.Namespace != default && type.FullName != default)
            {
                return type.Name + Suffix;
            }
            return fatherString + type.Name + Suffix;

        }
    }
}

