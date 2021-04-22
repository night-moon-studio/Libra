using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Libra.Extension.Utils
{
    public static class LibraResultHandler<S>
    {
        public readonly static Func<byte[], S> GetResult;
        static LibraResultHandler()
        {
            if (typeof(S).IsPrimitive || typeof(S).IsValueType)
            {
                GetResult = (obj) => JsonSerializer.Deserialize<LibraResult<S>>(obj).Value;
            }
            else if (typeof(S) == typeof(byte[]))
            {
                LibraResultHandler<byte[]>.GetResult = item => item;
            }
            else //if ()
            {
                GetResult = (obj) =>
                {
                    if (obj == null)
                    {
                        return default(S);
                    }
                    return JsonSerializer.Deserialize<S>(obj);
                };
            }
        }
    }


}
