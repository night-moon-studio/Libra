using Libra.Multicast;
using System;
using System.Collections.Concurrent;


public static class LibraMulticastHostManagement
{

    private static readonly ConcurrentDictionary<string, LibraMulticastHost> _keyHostMapper;
    private static readonly ConcurrentDictionary<string, Uri[]> _hostsCache;
    private static DynamicDictionaryBase<string, Uri[]> _keyUrlsMapper;
    static LibraMulticastHostManagement()
    {
        _keyHostMapper = new ConcurrentDictionary<string, LibraMulticastHost>();
        _hostsCache = new ConcurrentDictionary<string, Uri[]>();
        _keyUrlsMapper = _hostsCache.FuzzyTree();
    }

    /// <summary>
    /// 创建一个组播群
    /// </summary>
    /// <param name="multicastKey">KEY</param>
    /// <returns></returns>
    public static LibraMulticastHost GetOrCreate(string multicastKey)
    {
        if (!_keyHostMapper.TryGetValue(multicastKey,out var host))
        {
            host = new LibraMulticastHost(multicastKey);
            _keyHostMapper[multicastKey] = host;
            return host;
        }
        return host;
    }


    public static void SetMapper(string key, Uri[] urls)
    {
        _hostsCache[key] = urls;
        _keyUrlsMapper = _hostsCache.FuzzyTree();
    }

    public static Uri[] GetUrls(string key)
    {
        return _keyUrlsMapper[key];
    }

}

