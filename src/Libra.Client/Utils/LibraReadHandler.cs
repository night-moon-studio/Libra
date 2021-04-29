using Libra.Client.Protocal;
using System;
using System.Runtime.CompilerServices;

namespace Libra.Client.Utils
{

    /// <summary>
    /// 结果序列化程序
    /// </summary>
    /// <typeparam name="S"></typeparam>
    public static class LibraReadHandler<S>
    {

        public readonly static Func<byte[], S> GetResult;
        static LibraReadHandler()
        {

            if (typeof(S) != typeof(byte[]))
            {
                GetResult = LibraClientProtocal.ProtocalRead<S>();
            }
            else
            {
                Unsafe.AsRef(LibraReadHandler<byte[]>.GetResult) = item => item;
            }
            
        }
    }


}
