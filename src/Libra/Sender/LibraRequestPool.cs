using Libra;
using Libra.Model;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;



/// <summary>
/// Libra 操作池
/// </summary>
public static class LibraRequestPool
{

    private static string _baseUrl;
    private readonly static ConcurrentStack<LibraRequest> _stack;
    static LibraRequestPool()
    {
        _stack = new ConcurrentStack<LibraRequest>();
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
    internal static LibraRequest GetRequestInternal()
    {

        if (_stack.TryPop(out var client))
        {
            return client;
        }
        else
        {
            client = new LibraRequest();
            if (!string.IsNullOrEmpty(_baseUrl))
            {
                client.SetBaseUrl(_baseUrl);
            }
            return client;
        }

    }


    /// <summary>
    /// 异步执行并返回 byte[] 结果, 请求地址是 BaseUrl(可以通过 SetBaseUrl 进行配置)
    /// </summary>
    /// <param name="protocal">传递给对方服务器的协议内容</param>
    /// <returns></returns>
    public static async Task<byte[]> ExecuteAsync(LibraProtocal protocal)
    {
        return Execute(protocal);
    }


    /// <summary>
    /// 同步执行并返回对方执行的序列化结果, 请求地址是 BaseUrl (可以通过 SetBaseUrl 进行配置)
    /// </summary>
    /// <param name="protocal">传递给对方服务器的协议内容</param>
    /// <returns></returns>
    public static byte[] Execute(LibraProtocal protocal)
    {
        var request = GetRequestInternal();
        try
        {

            return request.GetMessage(protocal);

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
    /// 异步执行并返回状态码 (一般是对方的返回值为 void 时调用), 请求地址是 BaseUrl(可以通过 SetBaseUrl 进行配置)
    /// </summary>
    /// <param name="protocal">传递给对方服务器的协议内容</param>
    /// <returns></returns>
    public static async Task<HttpStatusCode> ExecuteVoidAsync(LibraProtocal protocal)
    {
        return ExecuteVoid(protocal);
    }


    /// <summary>
    /// 同步执行并返回状态码 (一般是对方的返回值为 void 时调用), 请求地址是 BaseUrl(可以通过 SetBaseUrl 进行配置)
    /// </summary>
    /// <param name="protocal">传递给对方服务器的协议内容</param>
    /// <returns></returns>
    public static HttpStatusCode ExecuteVoid(LibraProtocal protocal)
    {
        var request = GetRequestInternal();
        try
        {

            return request.GetHttpStatusCode(protocal);

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
    /// 异步执行并返回状态码 (一般是对方的返回值为 void 时调用), 请求地址是参数 URL
    /// </summary>
    /// <param name="url">请求地址(例如: http://xxxx/Libra )</param>
    /// <param name="protocal">传递给对方服务器的协议内容</param>
    /// <returns></returns>
    public static async Task<HttpStatusCode> ExecuteVoidAsync(Uri url, LibraProtocal protocal)
    {
        return ExecuteVoid(url, protocal);
    }


    /// <summary>
    /// 同步执行并返回状态码 (一般是对方的返回值为 void 时调用), 请求地址是参数 URL
    /// </summary>
    /// <param name="url">请求地址(例如: http://xxxx/Libra )</param>
    /// <param name="protocal">传递给对方服务器的协议内容</param>
    /// <returns></returns>
    public static HttpStatusCode ExecuteVoid(Uri url, LibraProtocal protocal)
    {
        var request = GetRequestInternal();
        try
        {

            return request.GetHttpStatusCode(url, protocal);

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
    /// 异步执行并返回对方执行的序列化结果 (一般是对方的返回值为 void 时调用), 请求地址是参数 URL
    /// </summary>
    /// <param name="url">请求地址(例如: http://xxxx/Libra )</param>
    /// <param name="protocal">传递给对方服务器的协议内容</param>
    /// <returns></returns>
    public static async Task<byte[]> ExecuteAsync(Uri url, LibraProtocal protocal)
    {
        return Execute(url, protocal);
    }


    /// <summary>
    /// 同步执行并返回对方执行的序列化结果 (一般是对方的返回值为 void 时调用), 请求地址是参数 URL
    /// </summary>
    /// <param name="url">请求地址(例如: http://xxxx/Libra )</param>
    /// <param name="protocal">传递给对方服务器的协议内容</param>
    /// <returns></returns>
    public static byte[] Execute(Uri url, LibraProtocal callModel)
    {

        var request = GetRequestInternal();
        try
        {

            return request.GetMessage(url, callModel);
           
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
    private static void Collect(LibraRequest request)
    {
        request.RefreshRequest();
        _stack.Push(request);
    }

}
