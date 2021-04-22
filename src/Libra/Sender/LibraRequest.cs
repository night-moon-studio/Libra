using Libra;
using Libra.Model;
using Natasha.CSharp;
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;

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

    public void SetBaseUrl(string baseUrl)
    {
        _request.RequestUri = new Uri(baseUrl + (baseUrl.EndsWith('/') ? "Libra" : "/Libra"));
    }


    private HttpResponseMessage GetReponse(LibraProtocal callModel)
    {
        _content.Protocal = callModel;
        return _client.SendAsync(_request, CancellationToken.None).Result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal HttpStatusCode GetHttpStatusCode(Uri url, LibraProtocal callModel)
    {
        _request.RequestUri = url;
        return GetHttpStatusCode(callModel);

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal HttpStatusCode GetHttpStatusCode(LibraProtocal callModel)
    {
        return GetReponse(callModel).StatusCode;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal byte[] GetMessage(Uri url, LibraProtocal callModel)
    {

        _request.RequestUri = url;
        return GetMessage(callModel);

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal byte[] GetMessage(LibraProtocal callModel)
    {

        var response = GetReponse(callModel);
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
        else if (response.StatusCode == HttpStatusCode.NotFound)
        {

            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                throw new Exception(response.Content.ReadAsStringAsync().Result);
            }
            else
            {
                throw new Exception("请检查对方服务是否开启!");
            }

        }
        else
        {

            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                throw new Exception("请求失败!" + response.Content.ReadAsStringAsync().Result);
            }
            else
            {
                throw new Exception("请求失败!");
            }

        }

    }

}
