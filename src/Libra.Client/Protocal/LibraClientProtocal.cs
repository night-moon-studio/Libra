using Libra.Model;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Libra.Client.Protocal
{
    public static class LibraClientProtocal
    {

        public static Func<T,Func<Stream,Task>> ProtocalWrite<T>()
        {
            if (typeof(T).IsPrimitive || typeof(T) == typeof(string) || typeof(T).IsValueType)
            {

                //基元类型及值类型使用 LibraSingleParameter 进行代理
                return obj => stream =>
                {
                    return JsonSerializer.SerializeAsync(stream, new LibraSingleParameter<T>() { Value = obj });
                };

            }
            else if (typeof(T) != typeof(byte[]))
            {
                //其他复杂类型
                return item => stream =>
                {
                    if (item == null)
                    {
                        return Task.CompletedTask;
                    }
                    return JsonSerializer.SerializeAsync(stream, item);
                };

            }
            return null;
        }

        public static Func<byte[], S> ProtocalRead<S>()
        {
            if (typeof(S).IsPrimitive || typeof(S) == typeof(string) || typeof(S).IsValueType )
            {

                //基元类型及值类型返回 LibraResult 代理的实体
                return bytes => JsonSerializer.Deserialize<LibraResult<S>>(bytes).Value;

            }
            else
            {

                //其他复杂类型
                return bytes =>
                {
                    if (bytes == null)
                    {
                        return default(S);
                    }
                    return JsonSerializer.Deserialize<S>(bytes);
                };

            }
        }
    }
}
