using Libra;
using Libra.Client.Utils;
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
    private int _retry;
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
    public LibraClient WithConfiguration(Uri uri, string route, string domain, Func<Stream, Task> protocal, Action<HttpRequestMessage> requestHandler, in CancellationToken cancellationToken,int retry)
    {
        if (uri != null)
        {
            _request.RequestUri = uri;
        }
        _retry = retry;
        _request.Headers.Add(LibraDefined.DOMAIN, domain);
        _request.Headers.Add(LibraDefined.ROUTE, route);
        _cancellationToken = cancellationToken;
        _content.ProtocalAction = protocal;
        requestHandler?.Invoke(_request);
        return this;
    }

    /// <summary>
    /// 取消令牌
    /// </summary>
    /// <param name="cancellationToken"></param>
    public LibraClient WithCancellation(in CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        return this;
    }

    /// <summary>
    /// 配置协议
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameters"></param>
    public LibraClient WithParameters<T>(T parameters)
    {
        _content.ProtocalAction = LibraWirteHandler<T>.Serialize(parameters);
        return this;
    }

    /// <summary>
    /// 配置重试次数
    /// </summary>
    /// <param name="retry"></param>
    public LibraClient WithRetry(int retry)
    {
        _retry = retry;
        return this;
    }

    /// <summary>
    /// 配置路由
    /// </summary>
    /// <param name="route"></param>
    public LibraClient WithRoute(string route)
    {
        _request.Headers.Add(LibraDefined.ROUTE, route);
        return this;
    }

    /// <summary>
    /// 设置请求域
    /// </summary>
    /// <param name="domain"></param>
    public LibraClient WithDomain(string domain)
    {
        _request.Headers.Add(LibraDefined.DOMAIN, domain);
        return this;
    }

    /// <summary>
    /// 配置协议委托
    /// </summary>
    /// <param name="protocal"></param>
    public LibraClient WithProtocal(Func<Stream, Task> protocal)
    {
        _content.ProtocalAction = protocal;
        return this;
    }

    /// <summary>
    /// 配置URIL
    /// </summary>
    /// <param name="uri"></param>
    public LibraClient WithUrl(Uri uri)
    {
        _request.RequestUri = uri;
        return this;
    }

    /// <summary>
    /// 配置请求
    /// </summary>
    /// <param name="requestHandler"></param>
    /// <returns></returns>
    public LibraClient WithRequest(Action<HttpRequestMessage> requestHandler)
    {
        requestHandler?.Invoke(_request);
        return this;
    }

    /// <summary>
    /// 刷新 Request 状态
    /// </summary>
    internal void RefreshRequest()
    {
        _retry = 0;
        _request.Headers.Clear();
        if (_request.RequestUri != _defaultUrl)
        {
            _request.RequestUri = _defaultUrl;
        }
        _content.ProtocalAction = item => Task.CompletedTask;
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
#if NET5_0_OR_GREATER
        socketHandler.EnableMultipleHttp2Connections = true;
#endif
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
    internal async Task<HttpContent> GetHttpContentAsync(int retry = 0)
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


        if (retry != _retry)
        {

            retry += 1;
            return await GetHttpContentAsync(retry);

        }


        //如果是 404 则认为目标地址没找到, 有可能是 URL 填写错误, 有可能是服务端不允许调用方法
        if (response.StatusCode == HttpStatusCode.NotFound)
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



    /// <summary>
    /// 指定地址执行返回实体
    /// </summary>
    /// <typeparam name="S">返回值类型</typeparam>
    /// <param name="url">远程服务的地址:应为 url + "/Libra"</param>
    /// <returns></returns>
    public async Task<S> GetResultAsync<S>()
    {

        var content = await GetHttpContentAsync().ConfigureAwait(false);
        return await LibraReadHandler<S>.GetResult(content);

    }

}
