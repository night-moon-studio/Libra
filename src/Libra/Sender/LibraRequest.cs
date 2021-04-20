using Libra.Model;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;

public static class LibraRequest
{

    private readonly static ConcurrentStack<HttpClient> _stack;
    static LibraRequest()
    {
        _stack = new ConcurrentStack<HttpClient>();
    }

    private static string _baseUrl;
    public static void SetBaseUrl(string baseUrl)
    {
        _baseUrl = baseUrl + (baseUrl.EndsWith('/') ? "Libra" : "/Libra");
    }
    internal static HttpClient GetClientInternal()
    {

        if (_stack.TryPop(out var client))
        {
            return client;
        }
        else
        {
            client = new HttpClient();
            return client;
        }

    }

    public static LibraResult<S> Execute<S>(LibraProtocalModel callModel)
    {
        var request = GetClientInternal();
        try
        {

            var response = GetMessage(request,_baseUrl, callModel);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode != HttpStatusCode.NoContent)
                {
                    var message = response.Content.ReadAsStringAsync().Result;
                    return JsonSerializer.Deserialize<LibraResult<S>>(message);
                }
                else
                {
                    throw new Exception($"{callModel.Flag} 暂不支持远程调用!");
                }
            }
            else 
            {
                throw new Exception("请求失败!");
            }

        }
        catch (Exception ex)
        {

            throw ex;

        }
        finally
        {
            _stack.Push(request);
        }
    }

    public static HttpStatusCode Execute(LibraProtocalModel callModel)
    {
        var request = GetClientInternal();
        try
        {

            var response = GetMessage(request, _baseUrl, callModel);
            return response.StatusCode;

        }
        catch (Exception ex)
        {

            throw ex;

        }
        finally
        {
            _stack.Push(request);
        }
    }

    public static HttpStatusCode Execute(string url, LibraProtocalModel callModel)
    {
        var request = GetClientInternal();
        try
        {
            var response = GetMessage(request, url + "/Libra", callModel);
            return response.StatusCode;

        }
        catch (Exception ex)
        {

            throw ex;

        }
        finally
        {
            _stack.Push(request);
        }
    }

    public static LibraResult<S> Execute<S>(string url, LibraProtocalModel callModel)
    {

        var request = GetClientInternal();
        try
        {

            var response = GetMessage(request, url + "/Libra", callModel);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode != HttpStatusCode.NoContent)
                {

                    var message = response.Content.ReadAsStringAsync().Result;
                    return JsonSerializer.Deserialize<LibraResult<S>>(message);
                }
                else 
                {
                    throw new Exception($"{callModel.Flag} 暂不支持远程调用!");
                }
            }
            else
            {
                throw new Exception("请求失败!");
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            _stack.Push(request);
        }

    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static HttpResponseMessage GetMessage(HttpClient request, string url, LibraProtocalModel callModel)
    {

        StringContent content = new StringContent(JsonSerializer.Serialize(callModel));
        content.Headers.ContentType.MediaType = "application/json";
        return request.PostAsync(url, content).Result;

    }


}
