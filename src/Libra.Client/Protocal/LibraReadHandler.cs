using Libra.Model;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace Libra.Client.Utils
{

    /// <summary>
    /// 结果序列化程序
    /// </summary>
    /// <typeparam name="S"></typeparam>
    public static class LibraReadHandler<S>
    {

        public readonly static Func<HttpContent, Task<S>> GetResult;
        static LibraReadHandler()
        {


            if (typeof(S).IsPrimitive || typeof(S).IsValueType)
            {

                //基元类型及值类型返回 LibraResult 代理的实体
                GetResult = async content => (await JsonSerializer.DeserializeAsync<LibraResult<S>>(await content.GetStreamAsync(), LibraJsonSettings.Options)).Value;

            }
            else if (typeof(S) == typeof(string))
            {

                Unsafe.AsRef(LibraReadHandler<string>.GetResult) = async content => await content.ReadAsStringAsync();

            }
            else if (typeof(S) == typeof(Stream))
            {

                Unsafe.AsRef(LibraReadHandler<Stream>.GetResult) = async content => await content.GetStreamAsync();

            }
            else if (typeof(S) == typeof(byte[]))
            {

                Unsafe.AsRef(LibraReadHandler<byte[]>.GetResult) = async content => await content.ReadAsByteArrayAsync();

            }
            else
            {
                //其他复杂类型
                GetResult = async content => 
                {

                    var stream = await content.GetStreamAsync();
                    if (stream.Length!=0)
                    {
                        return await JsonSerializer.DeserializeAsync<S>(stream, LibraJsonSettings.Options);
                    }
                    return default; 

                };

            }

        }
    }


}
