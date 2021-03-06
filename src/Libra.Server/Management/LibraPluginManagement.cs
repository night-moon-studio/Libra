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
public class LibraPluginManagement
{

    private readonly object _pluginLock = new object();
    private readonly ConcurrentDictionary<string, ConcurrentQueue<string>> _pluginKeyCache;
    private readonly ConcurrentDictionary<string, ConcurrentQueue<string>> _pluginTypesCache;
    private readonly ConcurrentDictionary<string, DomainBase> _nameDomainCache;
    private readonly ConcurrentDictionary<DomainBase, string> _domainPluginCache;
    private readonly string _domainName;
    public LibraPluginManagement(string domainName)
    {
        _domainName = domainName;
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
    public Assembly LoadPlugin(string path, params Type[] interfaces)
    {
        if (!_pluginKeyCache.ContainsKey(path))
        {
            lock (_pluginLock)
            {
                if (!_pluginKeyCache.ContainsKey(path))
                {
                    var domain = DomainManagement.Random;
                    var assembly = domain.LoadPlugin(path);
                    var types = assembly.GetTypes();

                    _domainPluginCache[domain] = path;
                    _pluginKeyCache[path] = new ConcurrentQueue<string>();
                    _pluginTypesCache[path] = new ConcurrentQueue<string>();
                    foreach (var item in types)
                    {
                        if (interfaces.Length == 0)
                        {
                            var typeName = item.GetRuntimeName();
                            _nameDomainCache[typeName] = domain;
                            _pluginTypesCache[path].Enqueue(typeName);
                        }
                        else
                        {
                            for (int i = 0; i < interfaces.Length; i += 1)
                            {
                                if (item.IsImplementFrom(interfaces[i]))
                                {
                                    var typeName = item.GetRuntimeName();
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
    public void AddRecoder(DomainBase domain, string key)
    {
        _pluginKeyCache[_domainPluginCache[domain]].Enqueue(key);
    }


    /// <summary>
    /// 通过类型名获取域
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public DomainBase GetPluginDominByType(string type)
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
    private string Clear(string pluginPath)
    {

        lock (_pluginLock)
        {
            if (_pluginKeyCache.TryGetValue(pluginPath, out var queue))
            {
                LibraMiddleware.Remove(_domainName, queue.ToArray());
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
    public bool UnloadPlugin(string pluginPath)
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

}

