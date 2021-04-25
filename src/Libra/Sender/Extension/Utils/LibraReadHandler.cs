using Libra.Protocal;
using System;
using System.Runtime.CompilerServices;

namespace Libra.Extension.Utils
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
                GetResult = LibraProtocal.ProtocalRead<S>();
            }
            else
            {
                Unsafe.AsRef(LibraReadHandler<byte[]>.GetResult) = item => item;
            }
            
        }
    }


}
