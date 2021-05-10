using Libra.Sender;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Libra 请求基本操作单元
/// </summary>
public class LibraClient
{

    private readonly static Action<HttpRequestMessage> _resetState;

    private static Uri _defaultUrl;
    private readonly HttpRequestMessage _request;
    private readonly HttpMessageInvoker _client;
    private readonly LibraContent _content;
    private CancellationToken _cancellationToken;
    static LibraClient()
    {


        var _state = typeof(HttpRequestMessage).GetField("_sendStatus", BindingFlags.NonPublic | BindingFlags.Instance);
        DynamicMethod method = new DynamicMethod(Guid.NewGuid().ToString(), null, new Type[] { typeof(HttpRequestMessage) });
        ILGenerator il = method.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldc_I4_0);
        il.Emit(OpCodes.Stfld, _state);
        il.Emit(OpCodes.Ret);
        _resetState = (Action<HttpRequestMessage>)(method.CreateDelegate(typeof(Action<HttpRequestMessage>)));



    }

    /// <summary>
    /// 配置请求信息
    /// </summary>
    public void ConfigClient(Uri uri, string route, Func<Stream, Task> protocal, Action<HttpRequestMessage> requestHandler, in CancellationToken cancellationToken)
    {
        if (uri != null)
        {
            _request.RequestUri = uri;
        }
        _content.ProtocalAction = protocal;
        _request.Headers.Add("Libra", route);
        _cancellationToken = cancellationToken;
        requestHandler?.Invoke(_request);
    }

    /// <summary>
    /// 配置协议委托
    /// </summary>
    /// <param name="protocal"></param>
    public void ConfigProtocal(Func<Stream, Task> protocal)
    {
        _content.ProtocalAction = protocal;
    }
    /// <summary>
    /// 配置URIL
    /// </summary>
    /// <param name="uri"></param>
    public void ConfigUrl(Uri uri)
    {
        _request.RequestUri = uri;
    }
    /// <summary>
    /// 配置请求
    /// </summary>
    /// <param name="requestHandler"></param>
    /// <returns></returns>
    public void ConfigRequest(Action<HttpRequestMessage> requestHandler)
    {
        requestHandler?.Invoke(_request);
    }
    /// <summary>
    /// 配置路由
    /// </summary>
    /// <param name="route"></param>
    public void ConfigRoute(string route)
    {
        _request.Headers.Add("Libra", route);
    }
    /// <summary>
    /// 刷新 Request 状态
    /// </summary>
    internal void RefreshRequest()
    {
        _request.Headers.Clear();
        if (_request.RequestUri != _defaultUrl)
        {
            _request.RequestUri = _defaultUrl;
        }
        _resetState(_request);
    }


    public LibraClient()
    {
        _content = new LibraContent();
        _request = new HttpRequestMessage(HttpMethod.Post, _defaultUrl);
        _request.Content = _content;

        var socketHandler = new SocketsHttpHandler();
        socketHandler.UseProxy = false;
        socketHandler.AllowAutoRedirect = false;
        socketHandler.AutomaticDecompression = DecompressionMethods.None;
        socketHandler.UseCookies = false;
        _client = new HttpMessageInvoker(socketHandler, disposeHandler: true);
    }



    /// <summary>
    /// 设置基础 URL
    /// </summary>
    /// <param name="baseUrl"></param>
    public void SetBaseUrl(string baseUrl)
    {
        _defaultUrl = new Uri(baseUrl);
        _request.RequestUri = _defaultUrl;
    }


    /// <summary>
    /// 根据协议内容获取 HttpReponse
    /// </summary>
    /// <param name="protocal">传递给对方服务器的协议内容</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task<HttpResponseMessage> GetReponseAsync()
    {
        return await _client.SendAsync(_request, _cancellationToken).ConfigureAwait(false);
    }


    /// <summary>
    /// 请求 URL 地址并获取状态码 (一般是对方的返回值为 void 时调用)
    /// </summary>
    /// <param name="url">请求地址(例如: http://xxxx )</param>
    /// <param name="protocal">传递给对方服务器的协议内容</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal async ValueTask<HttpStatusCode> GetResponseCodeAsync()
    {
        return (await GetReponseAsync().ConfigureAwait(false)).StatusCode;

    }

    /// <summary>
    /// 获取返回内容
    /// </summary>
    /// <param name="protocal">传递给对方服务器的协议内容</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal async Task<HttpContent> GetHttpContentAsync()
    {

        var response = await GetReponseAsync().ConfigureAwait(false);
        if (response.StatusCode == HttpStatusCode.OK)
        {

            return response.Content;

        }
        else if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return null;
        }
        //如果是 404 则认为目标地址没找到, 有可能是 URL 填写错误, 有可能是服务端不允许调用方法
        else if (response.StatusCode == HttpStatusCode.NotFound)
        {

            throw new Exception(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            //throw new Exception($"您当前的请求为 {_request.RequestUri} ;请检查地址及对方的服务是否开启!");

        }
        //如果是未知错误 则抛出异常
        else
        {

            throw new Exception("请求失败!" + await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            //throw new Exception($"您当前的请求为 {_request.RequestUri}, 请求失败!");

        }

    }

}
