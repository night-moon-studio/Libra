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
        _ltmCache = new ConcurrentDictionary<string, LibraTypeManagement>();
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


    public static LibraPluginManagement GetOrCreatePluginManagement(string domain)
    {

        if (_lpmCache.ContainsKey(domain))
        {
            return _lpmCache[domain];
        }
        else
        {
            var value = new LibraPluginManagement();
            _lpmCache[domain] = value;
            return value;
        }
    }

    public static void ConfigureFilter(string domain, Func<string, string, HttpRequest, HttpResponse, ValueTask<bool>> func)
    {
        LibraMiddleware.AddFilter(domain, func);
    }

}

