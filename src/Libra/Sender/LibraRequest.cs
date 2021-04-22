using Libra;
using Libra.Model;
using Libra.Sender;
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;


/// <summary>
/// Libra 请求基本操作单元
/// </summary>
public class LibraRequest
{
    private readonly static Func<HttpClient, HttpMethod, Uri, HttpRequestMessage> _createRequest;
    private readonly static Action<HttpRequestMessage> _resetState;
    private readonly static FieldInfo _state;
    private readonly static Uri _defaultUrl;
    private HttpRequestMessage _request;
    private readonly HttpClient _client;
    private readonly LibraContent _content;
    static LibraRequest()
    {

        var methodInfo = typeof(HttpClient).GetMethod("CreateRequestMessage", BindingFlags.NonPublic | BindingFlags.Instance);
        _createRequest = (Func<HttpClient, HttpMethod, Uri, HttpRequestMessage>)Delegate.CreateDelegate(typeof(Func<HttpClient, HttpMethod, Uri, HttpRequestMessage>), methodInfo);
        //_resetState = NDelegate
            //.RandomDomain()
            //.SetClass(item => item.AllowPrivate<HttpRequestMessage>())
            //.Action<HttpRequestMessage>("obj._sendStatus = 0;");

        _state= typeof(HttpRequestMessage).GetField("_sendStatus", BindingFlags.NonPublic | BindingFlags.Instance);
        _defaultUrl = new Uri("http://localhost:80");
    }

    /// <summary>
    /// 刷新 Request 状态
    /// </summary>
    internal void RefreshRequest()
    {

        _state.SetValue(_request,0);
        //_resetState(_request);
        //_request = _createRequest(_client, HttpMethod.Post, _request.RequestUri);
    }


    public LibraRequest()
    {
        _content = new LibraContent();
        _client = new HttpClient();
        _request = _createRequest(_client, HttpMethod.Post, _defaultUrl);
        _request.Content = _content;
    }


    /// <summary>
    /// 设置基础 URL
    /// </summary>
    /// <param name="baseUrl"></param>
    public void SetBaseUrl(string baseUrl)
    {
        _request.RequestUri = new Uri(baseUrl + (baseUrl.EndsWith('/') ? "Libra" : "/Libra"));
    }


    /// <summary>
    /// 根据协议内容获取 HttpReponse
    /// </summary>
    /// <param name="protocal">传递给对方服务器的协议内容</param>
    /// <returns></returns>
    private HttpResponseMessage GetReponse(LibraProtocal protocal)
    {
        _content.Protocal = protocal;
        return _client.SendAsync(_request, CancellationToken.None).Result;
    }


    /// <summary>
    /// 请求 URL 地址并获取状态码 (一般是对方的返回值为 void 时调用)
    /// </summary>
    /// <param name="url">请求地址(例如: http://xxxx/Libra )</param>
    /// <param name="protocal">传递给对方服务器的协议内容</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal HttpStatusCode GetHttpStatusCode(Uri url, LibraProtocal protocal)
    {
        _request.RequestUri = url;
        return GetHttpStatusCode(protocal);

    }


    /// <summary>
    /// 根据协议内容获取状态码, 目标地址为 BaseUrl 字段  (一般是对方的返回值为 void 时调用)
    /// </summary>
    /// <param name="protocal">传递给对方服务器的协议内容</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal HttpStatusCode GetHttpStatusCode(LibraProtocal protocal)
    {
        return GetReponse(protocal).StatusCode;
    }


    /// <summary>
    /// 请求 URL 地址并获取对方执行的序列化结果
    /// </summary>
    /// <param name="url">请求地址(例如: http://xxxx/Libra )</param>
    /// <param name="protocal">传递给对方服务器的协议内容</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal byte[] GetMessage(Uri url, LibraProtocal protocal)
    {

        _request.RequestUri = url;
        return GetMessage(protocal);

    }


    /// <summary>
    /// 请求 BaseUrl 地址并获取对方执行的序列化结果
    /// </summary>
    /// <param name="protocal">传递给对方服务器的协议内容</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal byte[] GetMessage(LibraProtocal protocal)
    {

        var response = GetReponse(protocal);
        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                return response.Content.ReadAsByteArrayAsync().Result;
            }
            else
            {
                return null;
            }
        }
        //如果是 404 则认为目标地址没找到, 有可能是 URL 填写错误, 有可能是服务端不允许调用方法
        else if (response.StatusCode == HttpStatusCode.NotFound)
        {

            //如果有内容则抛出内容
            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                throw new Exception(response.Content.ReadAsStringAsync().Result);
            }
            else
            {
                throw new Exception($"您当前的请求为 {_request.RequestUri} ;请检查地址及对方的服务是否开启!");
            }

        }
        //如果是未知错误 则抛出异常
        else
        {

            //如果有内容则输出内容
            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                throw new Exception("请求失败!" + response.Content.ReadAsStringAsync().Result);
            }
            else
            {
                throw new Exception($"您当前的请求为 {_request.RequestUri}, 请求失败!");
            }

        }

    }

}
