using Natasha;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


public static class LibraProxyClient
{
    private static readonly ConcurrentDictionary<string, Uri> _uriMapper;
    private static DynamicDictionaryBase<string, Uri> _fastUriMapper;
    static LibraProxyClient()
    {
        _uriMapper = new ConcurrentDictionary<string, Uri>();
        _fastUriMapper = _uriMapper.PrecisioTree();
    }

    public static T CreateClient<T>(string uri, 
        string domain = LibraDefined.DEFAULT_DOMAIN, 
        Action<HttpRequestMessage> action = null, 
        in CancellationToken token = default)
    {
        if (!_fastUriMapper.TryGetValue(domain, out var url))
        {
            url = new Uri(uri);
            _uriMapper[uri] = url;
        }
        return CreateClient<T>(url, domain, action, token);
    }

    public static T CreateClient<T>(Uri uri, string domain = LibraDefined.DEFAULT_DOMAIN, Action<HttpRequestMessage> action = null, in CancellationToken token = default)
    {

        if (!LibraProxyClient<T>._fastLibraProxyDelegateCache.TryGetValue(domain, out var func))
        {
            func = LibraProxyClient<T>.CreateDelegate(domain);
        }
        return func(uri, action, token);

    }

}

public static class LibraProxyClient<T>
{
    internal static readonly ConcurrentDictionary<string, Func<Uri, Action<HttpRequestMessage>, CancellationToken, T>> _libraProxyDelegateCache;
    internal static DynamicDictionaryBase<string, Func<Uri, Action<HttpRequestMessage>, CancellationToken, T>> _fastLibraProxyDelegateCache;
    static LibraProxyClient()
    {
        _libraProxyDelegateCache = new ConcurrentDictionary<string, Func<Uri, Action<HttpRequestMessage>, CancellationToken, T>>();
        _fastLibraProxyDelegateCache = _libraProxyDelegateCache.PrecisioTree();
    }

    public static Func<Uri, Action<HttpRequestMessage>, CancellationToken, T> CreateDelegate(string domain)
    {
        var proxier = new Proxier<T>();
        foreach (var item in proxier)
        {
            string script = default;
            var parameters = item.GetParameters();
            if (parameters.Length == 0)
            {
                script = $"new LibraExecutor(\"{typeof(T).GetRuntimeName()}.{item.Name}\",\"{domain}\", _cancellationToken, null)";
            }
            else if (parameters.Length == 1)
            {
                script = $"new LibraExecutor(\"{typeof(T).GetRuntimeName()}.{item.Name}\",\"{domain}\", _cancellationToken, LibraWirteHandler<{parameters[0].ParameterType.GetRuntimeName()}>.Serialize({parameters[0].Name}))";
            }
            else
            {
                var parameterScript = new StringBuilder();
                parameterScript.Append("new {");
                foreach (var parameter in item.GetParameters())
                {
                    parameterScript.Append($"{parameter.Name} = {parameter.Name},");
                }
                parameterScript.Length -= 1;
                parameterScript.Append('}');
                script = $"(\"{domain}\",\"{typeof(T).GetRuntimeName()}.{item.Name}\").WpcParam({parameterScript},_cancellationToken)";
            }

            Type returnType = item.ReturnType;
            if (returnType != typeof(void))
            {
                if (returnType.IsGenericType)
                {
                    if (returnType.GetGenericTypeDefinition() == typeof(Task<>) || returnType.GetGenericTypeDefinition() == typeof(ValueTask<>))
                    {

                        returnType = returnType.GetGenericArguments()[0];
                        if (returnType == typeof(HttpStatusCode))
                        {
                            script = $"return await {script}.GetCodeAsync(_uri,_requestAction).ConfigureAwait(false);";
                        }
                        else
                        {
                            script = $"return await {script}.GetResultAsync<{returnType.GetDevelopName()}>(_uri,_requestAction).ConfigureAwait(false);";
                        }

                    }
                }
                else if (returnType == typeof(Task) || returnType.BaseType == typeof(ValueTask))
                {
                    script = $"await {script}.GetCodeAsync(_uri,_requestAction).ConfigureAwait(false);";
                }
                else
                {
                    if (returnType == typeof(HttpStatusCode))
                    {
                        script = $"return {script}.GetCodeAsync(_uri,_requestAction).Result;";
                    }
                    else
                    {
                        script = $"return {script}.GetResultAsync<{returnType.GetDevelopName()}>(_uri,_requestAction).Result;";
                    }
                }
            }
            else
            {
                script = $"{script}.GetCodeAsync(_uri,_requestAction).Result;";
            }
            proxier[item.Name] = script;
        }
        proxier.ClassBuilder.Ctor(item => item
            .Public()
            .Param<Uri>("uri")
            .Param<Action<HttpRequestMessage>>("requestAction")
            .Param<CancellationToken>("cts", "in ")
            .Body(@"
                        _uri = uri;
                        _requestAction = requestAction;
                        _cancellationToken = cts;
                ")
        );
        proxier.ClassBuilder.PrivateReadonlyField<Uri>("_uri");
        proxier.ClassBuilder.PrivateReadonlyField<Action<HttpRequestMessage>>("_requestAction");
        proxier.ClassBuilder.PrivateReadonlyField<CancellationToken>("_cancellationToken");
        var func = proxier.GetCreator<Uri, Action<HttpRequestMessage>, CancellationToken>();
        _libraProxyDelegateCache[domain] = func;
        _fastLibraProxyDelegateCache = _libraProxyDelegateCache.PrecisioTree();
        return func;
    }
}


