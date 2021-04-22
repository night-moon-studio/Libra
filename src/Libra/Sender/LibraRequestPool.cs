using Libra.Model;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

public static class LibraRequestPool
{

    private readonly static ConcurrentStack<LibraRequest> _stack;
    static LibraRequestPool()
    {
        _stack = new ConcurrentStack<LibraRequest>();
    }

    private static string _baseUrl;
    public static void SetBaseUrl(string baseUrl)
    {
        _baseUrl = baseUrl;
    }
    internal static LibraRequest GetClientInternal()
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


    public static async Task<byte[]> ExecuteAsync(LibraProtocal callModel)
    {
        return Execute(callModel);
    }
    public static byte[] Execute(LibraProtocal callModel)
    {
        var request = GetClientInternal();
        try
        {

            return request.GetMessage(callModel);

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

    public static async Task<HttpStatusCode> ExecuteVoidAsync(LibraProtocal callModel)
    {
        return ExecuteVoid(callModel);
    }
    public static HttpStatusCode ExecuteVoid(LibraProtocal callModel)
    {
        var request = GetClientInternal();
        try
        {

            return request.GetHttpStatusCode(callModel);

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


    public static async Task<HttpStatusCode> ExecuteVoidAsync(Uri url, LibraProtocal callModel)
    {
        return ExecuteVoid(url, callModel);
    }
    public static HttpStatusCode ExecuteVoid(Uri url, LibraProtocal callModel)
    {
        var request = GetClientInternal();
        try
        {

            return request.GetHttpStatusCode(url, callModel);

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


    public static async Task<byte[]> ExecuteAsync(Uri url, LibraProtocal callModel)
    {
        return Execute(url, callModel);
    }

    public static byte[] Execute(Uri url, LibraProtocal callModel)
    {

        var request = GetClientInternal();
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

    private static void Collect(LibraRequest request)
    {
        request.RefreshRequest();
        _stack.Push(request);
    }

}
