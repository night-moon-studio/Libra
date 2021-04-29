using Libra.Client.Protocal;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Libra.Client.Utils
{


    /// <summary>
    /// 参数处理程序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LibraWriteHandler<T> : LibraExecutor
    {

        public LibraWriteHandler(string route, T parameter) : base(route, _serialize(parameter))
        {

        }


        private static readonly Func<T, Func<Stream, Task>> _serialize;
        static LibraWriteHandler()
        {

            if (typeof(T) != typeof(byte[]))
            {
                _serialize = LibraClientProtocal.ProtocalWrite<T>();
            }
            else
            {
                Unsafe.AsRef(LibraWriteHandler<byte[]>._serialize) = obj => stream =>
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
