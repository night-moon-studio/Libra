using Libra.Model;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

public static class LibraRequest
{

    private readonly static ConcurrentStack<HttpClient> _stack;
    static LibraRequest()
    {
        _stack = new ConcurrentStack<HttpClient>();
    }

    private static Uri _baseUrl;
    public static void SetBaseUrl(string baseUrl)
    {
        _baseUrl = new Uri(baseUrl + (baseUrl.EndsWith('/') ? "Libra" : "/Libra"));
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


    public static async Task<string> ExecuteAsync(LibraProtocal callModel)
    {
        return Execute(callModel);
    }
    public static string Execute(LibraProtocal callModel)
    {
        var request = GetClientInternal();
        try
        {

            var response = GetMessage(request,_baseUrl, callModel);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode != HttpStatusCode.NoContent)
                {
                    return response.Content.ReadAsStringAsync().Result;
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
        catch (Exception ex)
        {

            throw ex;

        }
        finally
        {
            _stack.Push(request);
        }
    }

    public static async Task<HttpStatusCode> ExecuteVoidAsync(LibraProtocal callModel)
    {
        return ExecuteVoid(callModel);
    }
    public static HttpStatusCode ExecuteVoid(LibraProtocal callModel)
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


    public static async Task<HttpStatusCode> ExecuteVoidAsync(Uri url, LibraProtocal callModel)
    {
        return ExecuteVoid(url, callModel);
    }
    public static HttpStatusCode ExecuteVoid(Uri url, LibraProtocal callModel)
    {
        var request = GetClientInternal();
        try
        {
            var response = GetMessage(request, url, callModel);
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


    public static async Task<string> ExecuteAsync(Uri url, LibraProtocal callModel)
    {
        return Execute(url, callModel);
    }
    public static string Execute(Uri url, LibraProtocal callModel)
    {

        var request = GetClientInternal();
        try
        {

            var response = GetMessage(request, url, callModel);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode != HttpStatusCode.NoContent)
                {
                    return response.Content.ReadAsStringAsync().Result;
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
                    throw new Exception("请求失败!"+ response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    throw new Exception("请求失败!");
                }

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
    private static HttpResponseMessage GetMessage(HttpClient request, Uri url, LibraProtocal callModel)
    {

        StringContent content = new StringContent(JsonSerializer.Serialize(callModel));
        content.Headers.ContentType.MediaType = "application/json";
        return request.PostAsync(url, content).Result;

    }


}
