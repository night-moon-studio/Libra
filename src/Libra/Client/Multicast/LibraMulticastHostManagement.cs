using Libra.Multicast;
using System;
using System.Collections.Concurrent;


/// <summary>
/// Libra 多播管理类
/// </summary>
public static class LibraMulticastHostManagement
{

    private static readonly ConcurrentDictionary<string, LibraMulticastHost> _keyHostMapper;
    private static readonly ConcurrentDictionary<string, Uri[]> _hostsCache;
    private static DynamicDictionaryBase<string, Uri[]> _keyUrlsMapper;
    static LibraMulticastHostManagement()
    {
        NatashaInitializer.InitializeAndPreheating();
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


    /// <summary>
    /// 设置 多播KEY 到 URL 的映射. 一个 KEY 对应多个 URL.
    /// </summary>
    /// <param name="key">多播KEY</param>
    /// <param name="urls">目标url</param>
    public static void SetMapper(string key, Uri[] urls)
    {
        _hostsCache[key] = urls;
        _keyUrlsMapper = _hostsCache.FuzzyTree();
    }

    /// <summary>
    /// 根据 多播KEY 获取URL地址集合
    /// </summary>
    /// <param name="key">多播KEY</param>
    /// <returns></returns>
    public static Uri[] GetUrls(string key)
    {
        return _keyUrlsMapper[key];
    }

}

