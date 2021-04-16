using Libra.Model;
using System;
using System.Collections.Concurrent;
using System.Net.Http;
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
        _baseUrl = baseUrl;
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
            _stack.Push(client);
            return client;
        }

    }

    public static LibraResult<S> Post<S>(LibraProtocalModel callModel)
    {
        return Post<S>(_baseUrl, callModel);
    }
    public static LibraResult<S> Post<S>(string url, LibraProtocalModel callModel)
    {

        var request = GetClientInternal();
        try
        {
            
            StringContent content = new StringContent(JsonSerializer.Serialize(callModel));
            content.Headers.ContentType.MediaType = "application/json";
            var result = request.PostAsync(url + "api/Libra", content).Result;
            if (result.IsSuccessStatusCode)
            {
                if (result.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    var message = result.Content.ReadAsStringAsync().Result;
                    if (message == "该类型不支持远程调用!")
                    {
                        throw new Exception($"{callModel.Flag} 暂不支持远程调用!");
                    }
                    return JsonSerializer.Deserialize<LibraResult<S>>(message);
                }
            }
            return default;

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

}
