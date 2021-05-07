using Libra.Client.Protocal;
using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Libra.Client.Utils
{

    public static class LibraWirteHandler<T> 
    {
        public static readonly Func<T, Func<Stream, Task>> Serialize;
        static LibraWirteHandler()
        {

            if (typeof(T) != typeof(byte[]))
            {
                Serialize = LibraClientProtocal.ProtocalWrite<T>();
            }
            else
            {
                Unsafe.AsRef(LibraWirteHandler<byte[]>.Serialize) = obj => stream =>
                {
                    if (obj == null)
                    {
                        return Task.CompletedTask;
                    }
                    return stream.WriteAsync(obj, 0, obj.Length);
                };
            }
        }
    }


    
}
