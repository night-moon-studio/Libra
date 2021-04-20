using Libra.Model;
using System;
using System.Net;
using System.Text.Json;

public class LibraProtocalWrapper
{

    private readonly LibraProtocalModel _callMode;
    public LibraProtocalWrapper(string caller)
    {
        _callMode = new LibraProtocalModel() { Flag = caller, Parameters = null };
    }

    public S Get<S>(string url)
    {
        var result = LibraRequest.Execute<S>(url, _callMode);
        return result == null ? default : result.Value;
    }


    public S Get<S>()
    {
        var result = LibraRequest.Execute<S>(_callMode);
        return result == null ? default : result.Value;
    }

    public HttpStatusCode Execute()
    {
        return LibraRequest.Execute(_callMode);
    }

}


public class LibraProtocalWrapper<T>
{

    private static readonly Func<T, string> _serialize;
    static LibraProtocalWrapper()
    {
        if (typeof(T).IsPrimitive || typeof(T) == typeof(string) || typeof(T) == typeof(DateTime))
        {
            _serialize = (obj) => JsonSerializer.Serialize(new LibraSingleParameter<T>() { Value = obj });
        }
        else
        {
            _serialize = (obj) =>
            {
                if (obj == null)
                {
                    return "";
                }
                return JsonSerializer.Serialize(obj);
            };
        }

    }


    private readonly LibraProtocalModel _callMode;
    public LibraProtocalWrapper(string caller, T parameter)
    {
        _callMode = new LibraProtocalModel() { Flag = caller, Parameters = _serialize(parameter) };
    }


    public S Get<S>(string url)
    {
        var result = LibraRequest.Execute<S>(url, _callMode);
        return result == null ? default : result.Value;
    }


    public S Get<S>()
    {
        var result = LibraRequest.Execute<S>(_callMode);
        return result == null ? default : result.Value;
    }

    public HttpStatusCode Execute()
    {
        return LibraRequest.Execute(_callMode);
    }

}