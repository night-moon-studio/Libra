using Libra.Model;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Libra.Client.Utils
{

    public static class LibraWirteHandler<T>
    {
        public static readonly Func<T, Func<Stream, Task>> Serialize;
        static LibraWirteHandler()
        {

            if (typeof(T).IsPrimitive || typeof(T).IsValueType)
            {

                //基元类型及值类型使用 LibraSingleParameter 进行代理
                Serialize = obj => async stream =>
                {
                    await JsonSerializer.SerializeAsync(stream, new LibraSingleParameter<T>() { Value = obj });
                };

            }
            else if (typeof(T) == typeof(string))
            {
                Unsafe.AsRef(LibraWirteHandler<string>.Serialize) = obj =>
                {
                    if (string.IsNullOrEmpty(obj))
                    {
                        return async stream => { };
                    }
                    else
                    {
                        return async stream =>
                        {
                            await stream.WriteAsync(Encoding.UTF8.GetBytes(obj));
                        };
                    }
                };
            }
            else if (typeof(T) == typeof(byte[]))
            {
                Unsafe.AsRef(LibraWirteHandler<byte[]>.Serialize) = obj =>
                {

                    if (obj == null)
                    {
                        return async stream => { };
                    }
                    else
                    {
                        return async stream =>
                        {
                            await stream.WriteAsync(obj, 0, obj.Length);
                        };
                    }

                };


            }
            else if (typeof(T) == typeof(Stream))
            {
                Unsafe.AsRef(LibraWirteHandler<Stream>.Serialize) = obj =>
                {
                    if (obj == null)
                    {
                        return async stream => { };
                    }
                    else
                    {
                        return async stream =>
                        {
                            await stream.CopyToAsync(stream);
                        };
                    }
                };
            }
            else
            {
                //其他复杂类型
                Serialize = obj =>
                {
                    if (obj == null)
                    {
                        return async stream => { };
                    }
                    else
                    {
                        return async stream =>
                        {
                            await JsonSerializer.SerializeAsync(stream, obj);
                        };
                    }

                };

            }
        }
    }

}
