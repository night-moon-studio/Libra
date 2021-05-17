using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class LibraDomainManagement
{
    private readonly static ConcurrentDictionary<string, LibraPluginManagement> _lpmCache;
    private readonly static ConcurrentDictionary<string, LibraTypeManagement> _ltmCache;

   
    static LibraDomainManagement()
    {
        _lpmCache = new ConcurrentDictionary<string, LibraPluginManagement>();
        _lpmCache[LibraDefined.DEFAULT_DOMAIN] = new LibraPluginManagement(LibraDefined.DEFAULT_DOMAIN);
        _ltmCache = new ConcurrentDictionary<string, LibraTypeManagement>();
        _ltmCache[LibraDefined.DEFAULT_DOMAIN] = new LibraTypeManagement();
    }

    public static LibraTypeManagement GetDefaultTypeManagement()
    {
        return _ltmCache[LibraDefined.DEFAULT_DOMAIN];
    }
    public static LibraTypeManagement GetOrCreateTypeManagement(string domain)
    {

        if (_ltmCache.ContainsKey(domain))
        {
            return _ltmCache[domain];
        }
        else
        {
            var value = new LibraTypeManagement();
            _ltmCache[domain] = value;
            return value;
        }
    }

    public static LibraPluginManagement GetDefaultPluginManagement()
    {
        return _lpmCache[LibraDefined.DEFAULT_DOMAIN];
    }

    public static LibraPluginManagement GetOrCreatePluginManagement(string domain)
    {

        if (_lpmCache.ContainsKey(domain))
        {
            return _lpmCache[domain];
        }
        else
        {
            var value = new LibraPluginManagement(domain);
            _lpmCache[domain] = value;
            return value;
        }
    }

    /// <summary>
    /// 将插件装在到默认域中
    /// </summary>
    /// <param name="path"></param>
    public static void LoadPlugin(string path)
    {
        var management = GetDefaultPluginManagement();
        management.LoadPlugin(path);
    }
    /// <summary>
    /// 将某插件装在某域中,以供远程调用
    /// </summary>
    /// <param name="domain"></param>
    /// <param name="path"></param>
    public static void LoadPlugin(string domain, string path)
    {
        var management = GetOrCreatePluginManagement(domain);
        management.LoadPlugin(path);
    }


    /// <summary>
    /// 将某插件装在某域中,并挑选出被约束的类作为对外开放类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="domain"></param>
    /// <param name="path"></param>
    public static void LoadPlugin<T>(string domain, string path)
    {
        var management = GetOrCreatePluginManagement(domain);
        management.LoadPlugin(path,typeof(T));
    }


    /// <summary>
    /// 卸载对应域的插件
    /// </summary>
    /// <param name="domain"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool UnloadPlugin(string domain, string path)
    {
        var management = GetOrCreatePluginManagement(domain);
        return management.UnloadPlugin(path);
    }


    /// <summary>
    /// 卸载默认域的插件
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool UnloadPlugin(string path)
    {
        var management = GetDefaultPluginManagement();
        return management.UnloadPlugin(path);
    }


    /// <summary>
    /// 配置对应域的过滤器
    /// </summary>
    /// <param name="domain"></param>
    /// <param name="func"></param>
    public static void ConfigureFilter(string domain, Func<string, string, HttpRequest, HttpResponse, ValueTask<bool>> func)
    {
        LibraMiddleware.AddFilter(domain, func);
    }


    // <summary>
    /// 配置默认域的过滤器
    /// </summary>
    /// <param name="func"></param>
    public static void ConfigureFilter(Func<string, string, HttpRequest, HttpResponse, ValueTask<bool>> func)
    {
        LibraMiddleware.AddFilter(LibraDefined.DEFAULT_DOMAIN, func);
    }
}

