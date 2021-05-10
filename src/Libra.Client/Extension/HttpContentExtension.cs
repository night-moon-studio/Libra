using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


public static class HttpContentExtension
{
    private readonly static Func<HttpContent, long, CancellationToken, Task> _writeBuffer;
    private readonly static Func<HttpContent, Stream> _getStream;

    static HttpContentExtension()
    {

        var _dealInBufferMethodInfo = typeof(HttpContent).GetMethod("LoadIntoBufferAsync", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(long), typeof(CancellationToken) }, null);
        _writeBuffer = (Func<HttpContent, long, CancellationToken, Task>)(Delegate.CreateDelegate(typeof(Func<HttpContent, long, CancellationToken, Task>), _dealInBufferMethodInfo));
        var _bufferedContentFieldInfo = typeof(HttpContent).GetField("_bufferedContent", BindingFlags.NonPublic | BindingFlags.Instance);
        var method = new DynamicMethod(Guid.NewGuid().ToString(), typeof(Stream), new Type[] { typeof(HttpContent) });
        var il = method.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, _bufferedContentFieldInfo);
        il.Emit(OpCodes.Ret);
        _getStream = (Func<HttpContent, Stream>)(method.CreateDelegate(typeof(Func<HttpContent, Stream>)));

    }
    public static async Task<Stream> GetStreamAsync(this HttpContent httpContent)
    {
        await _writeBuffer(httpContent, int.MaxValue, CancellationToken.None);
        return _getStream(httpContent);
    }

}

