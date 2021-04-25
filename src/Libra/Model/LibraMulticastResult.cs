using System.Net;


public class LibraMulticastResult
{
    public LibraMulticastResult(string url, HttpStatusCode result)
    {
        Url = url;
        Result = result;
    }
    /// <summary>
    /// 请求地址
    /// </summary>
    public readonly string Url;
    /// <summary>
    /// 请求结果
    /// </summary>
    public readonly HttpStatusCode Result;
}


/// <summary>
/// 多播结果
/// </summary>
/// <typeparam name="S"></typeparam>
public class LibraMulticastResult<S>
{
    public LibraMulticastResult(string url, S result)
    {
        Url = url;
        Result = result;
    }
    /// <summary>
    /// 请求地址
    /// </summary>
    public readonly string Url;
    /// <summary>
    /// 请求结果
    /// </summary>
    public readonly S Result;
}

