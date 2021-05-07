using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Libra.Client.Utils.Extension
{

    public static class ReadAndWriteExtension
    {

        public static Func<Stream, Task> GetParameterFunc<T>(this T parameters)
        {
            return LibraWirteHandler<T>.Serialize(parameters);
        }

        public static S GetResultFromBytes<S>(this byte[] parameters)
        {
            return LibraReadHandler<S>.GetResult(parameters);
        }

    }

}
