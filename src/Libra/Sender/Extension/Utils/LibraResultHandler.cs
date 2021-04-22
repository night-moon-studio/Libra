using System;
using System.Text.Json;

namespace Libra.Extension.Utils
{

    /// <summary>
    /// 结果序列化程序
    /// </summary>
    /// <typeparam name="S"></typeparam>
    public static class LibraResultHandler<S>
    {

        public readonly static Func<byte[], S> GetResult;
        static LibraResultHandler()
        {
            if (typeof(S).IsPrimitive || typeof(S).IsValueType) 
            {

                //基元类型及值类型返回 LibraResult 代理的实体
                GetResult = (obj) => JsonSerializer.Deserialize<LibraResult<S>>(obj).Value;

            }
            else if (typeof(S) == typeof(byte[]))   
            {

                //byte[] 直接返回
                LibraResultHandler<byte[]>.GetResult = item => item;

            }
            else 
            {

                //其他复杂类型
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
