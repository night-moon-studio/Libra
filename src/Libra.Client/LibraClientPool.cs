using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
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
    public static void SetBaseUrl(string baseUrl)
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
    /// 同步执行并返回对方执行的序列化结果, 请求地址是 BaseUrl (可以通过 SetBaseUrl 进行配置)
    /// </summary>
    /// <param name="protocal">传递给对方服务器的协议内容</param>
    /// <returns></returns>
    public static byte[] BytesResult(string route, Func<Stream, Task> protocal)
    {
        var request = GetRequestInternal();
        try
        {

            return request.GetResponseBytes(route, protocal);

        }
        catch (Exception ex)
        {

            throw ex;

        }
        finally
        {
            //回收客户端
            Collect(request);
        }
    }


    /// <summary>
    /// 同步执行并返回状态码 (一般是对方的返回值为 void 时调用), 请求地址是 BaseUrl(可以通过 SetBaseUrl 进行配置)
    /// </summary>
    /// <param name="protocal">传递给对方服务器的协议内容</param>
    /// <returns></returns>
    public static HttpStatusCode CodeResult(string route, Func<Stream, Task> protocal)
    {
        var request = GetRequestInternal();
        try
        {

            return request.GetResponseCode(route, protocal);

        }
        catch (Exception ex)
        {

            throw ex;

        }
        finally
        {
            Collect(request);
        }
    }


    /// <summary>
    /// 同步执行并返回状态码 (一般是对方的返回值为 void 时调用), 请求地址是参数 URL
    /// </summary>
    /// <param name="url">请求地址(例如: http://xxxx )</param>
    /// <param name="protocal">传递给对方服务器的协议内容</param>
    /// <returns></returns>
    public static HttpStatusCode CodeResult(Uri url, string route, Func<Stream, Task> protocal)
    {
        var request = GetRequestInternal();
        try
        {

            return request.GetResponseCode(url, route, protocal);

        }
        catch (Exception ex)
        {

            throw ex;

        }
        finally
        {

            Collect(request);
        }
    }


    /// <summary>
    /// 同步执行并返回对方执行的序列化结果 (一般是对方的返回值为 void 时调用), 请求地址是参数 URL
    /// </summary>
    /// <param name="url">请求地址(例如: http://xxxx )</param>
    /// <param name="protocal">传递给对方服务器的协议内容</param>
    /// <returns></returns>
    public static byte[] BytesResult(Uri url, string route, Func<Stream, Task> protocal)
    {

        var request = GetRequestInternal();
        try
        {

            return request.GetResponseBytes(url, route, protocal);
           
        }
        catch (Exception ex)
        {

            throw ex;

        }
        finally
        {

            Collect(request);

        }

    }


    /// <summary>
    /// 重置并回收 Request
    /// </summary>
    /// <param name="request"></param>
    private static void Collect(LibraClient request)
    {
        request.RefreshRequest();
        _stack.Push(request);
    }

}
