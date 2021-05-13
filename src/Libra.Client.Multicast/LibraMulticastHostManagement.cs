using Libra.Client.Multicast;
using System;
using System.Collections.Concurrent;
using System.Net.Http;

/// <summary>
/// Libra 多播管理类
/// </summary>
public static class LibraMulticastHostManagement
{

    private static readonly ConcurrentDictionary<string, LibraMulticastHost> _keyHostMapper;
    private static readonly ConcurrentDictionary<string, LibraMulticastModel[]> _hostsCache;
    private static DynamicDictionaryBase<string, LibraMulticastModel[]> _keyUrlsMapper;
    static LibraMulticastHostManagement()
    {
        NatashaInitializer.InitializeAndPreheating();
        _keyHostMapper = new ConcurrentDictionary<string, LibraMulticastHost>();
        _hostsCache = new ConcurrentDictionary<string, LibraMulticastModel[]>();
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
            _hostsCache[multicastKey] = null;
            _keyUrlsMapper = _hostsCache.FuzzyTree();
            return host;
        }
        return host;
    }


    /// <summary>
    /// 设置 多播KEY 到 URL 的映射. 一个 KEY 对应多个 URL.
    /// </summary>
    /// <param name="key">多播KEY</param>
    /// <param name="urls">目标url</param>
    public static void SetMapper(string key, LibraMulticastModel[] urls)
    {
        _hostsCache[key] = urls;
        _keyUrlsMapper.Change(key, urls);
    }

    /// <summary>
    /// 根据 多播KEY 获取URL地址集合
    /// </summary>
    /// <param name="key">多播KEY</param>
    /// <returns></returns>
    public static LibraMulticastModel[] GetUrls(string key)
    {
        return _keyUrlsMapper[key];
    }

}

