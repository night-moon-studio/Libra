using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;



/// <summary>
/// Libra 操作池
/// </summary>
public static class LibraClientPool
{

    private static string _baseUrl;
    private readonly static ConcurrentStack<LibraClient> _stack;

    static LibraClientPool()
    {
        _stack = new ConcurrentStack<LibraClient>();
    }

    /// <summary>
    /// 设置全局 url. LibraRequest 在初始化时将使用该地址
    /// </summary>
    /// <param name="baseUrl"></param>
    public static void SetGlobalBaseUrl(string baseUrl)
    {
        _baseUrl = baseUrl;
    }


    /// <summary>
    /// 从池中获取一个可用的 客户端
    /// </summary>
    /// <returns></returns>
    internal static LibraClient GetRequestInternal()
    {

        if (_stack.TryPop(out var client))
        {
            return client;
        }
        else
        {
            client = new LibraClient();
            if (!string.IsNullOrEmpty(_baseUrl))
            {
                client.SetBaseUrl(_baseUrl);
            }
            return client;
        }

    }
   
    /// <summary>
    /// 重置并回收 Request
    /// </summary>
    /// <param name="request"></param>
    public static void Collect(LibraClient request)
    {
        request.RefreshRequest();
        _stack.Push(request);
    }

}
