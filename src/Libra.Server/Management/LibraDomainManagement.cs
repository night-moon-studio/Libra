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
    public static void RemovePluginManagement(string domain)
    {
        var management = GetOrCreatePluginManagement(domain);
        _lpmCache.Remove(domain);
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

    public static void ConfigureFilter(string domain, Func<string, string, HttpRequest, HttpResponse, ValueTask<bool>> func)
    {
        LibraMiddleware.AddFilter(domain, func);
    }

}

