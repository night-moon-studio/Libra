using System;
using System.IO;
using System.Threading.Tasks;

namespace Libra.Server.Protocal
{
    public static class LibraWriteHandler
    {
        /// <summary>
        /// 根据返回值类型,获取返回脚本
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetReturnScript(Type returnType, string methodCaller, bool isAsync)
        {
            if (returnType == typeof(void) || returnType == typeof(Task) || returnType == typeof(ValueTask))
            {

                //如果返回值为 void 或者是 Task
                //生成执行逻辑代码:
                // [await] (new TestService()).Hello(parameters.Name,parameters.Age);
                return $"{(isAsync ? "await" : "")} {methodCaller};";

            }
            else if (returnType == typeof(string))
            {
                //如果返回值为字符串类型
                //生成执行逻辑代码:
                //var result =[await] (new TestService()).Hello(parameters.Name,parameters.Age)[.ConfigureAwait(false)]);
                //if(!string.IsNullOrEmpty(result))
                //{
                //  await response.WriteAsync(result);
                //}   
                string result = $"var result = {(isAsync ? "await" : "")} {methodCaller}{(isAsync ? ".ConfigureAwait(false)" : "")};";
                return result + $"if(!string.IsNullOrEmpty(result)) {{ await response.WriteAsync(result); }}";

            }
            else if (returnType.IsPrimitive || returnType.IsValueType)
            {

                //如果返回值为基元类型或者值类型
                //生成执行逻辑代码:
                // var result =  new LibraResult<int>(){ Value = [await] (new TestService()).Hello(parameters.Name,parameters.Age)[.ConfigureAwait(false)] };
                // await JsonSerializer.SerializeAsync((response.Body,result);
                string result = $"var result = new LibraResult<{returnType.GetDevelopName()}>(){{ Value = {(isAsync ? "await" : "")} {methodCaller}{(isAsync ? ".ConfigureAwait(false)" : "")}}};";
                return result + $"await System.Text.Json.JsonSerializer.SerializeAsync(response.Body,result).ConfigureAwait(false);";


            }
            else if (returnType == typeof(byte[]))
            {

                //如果是byte[]类型,则直接返回
                //生成执行逻辑代码:
                //var result =[await] (new TestService()).Hello(parameters.Name,parameters.Age)[.ConfigureAwait(false)]);
                //if(result!=null && result.Length!=0)
                //{
                //  await response.BodyWriter.WriteAsync(result);
                //}
                string result = $"var result = {(isAsync ? "await" : "")} {methodCaller}{(isAsync ? ".ConfigureAwait(false)" : "")};";
                return result + $"if(result!=null && result.Length!=0) {{ await response.BodyWriter.WriteAsync(result); }}";


            }
            else if (returnType == typeof(Stream))
            {

                //如果是Stream类型,则使用流写入
                //生成执行逻辑代码:
                //var stream =[await] (new TestService()).Hello(parameters.Name,parameters.Age)[.ConfigureAwait(false)]);
                //if(stream!=null && stream.Length!=0)
                //{
                //      await stream.CopyToAsync(response.BodyWriter);
                //}

                string result = $"var result = {(isAsync ? "await" : "")} {methodCaller}{(isAsync ? ".ConfigureAwait(false)" : "")};";
                return result + $"if(result!=null && result.Length!=0) {{ await result.CopyToAsync(response.BodyWriter); }}";

            }
            else
            {

                //如果是其他墙类型
                //生成执行逻辑代码:
                // var result = [await] (new TestService()).Hello(parameters.Name,parameters.Age)[.ConfigureAwait(false)];
                //if(result!=default)
                //{
                //  await System.Text.Json.JsonSerializer.SerializeAsync(response.Body,result).ConfigureAwait(false);
                //}
                var result = $"var result = {(isAsync ? "await" : "")} {methodCaller}{(isAsync ? ".ConfigureAwait(false)" : "")};";
                return result + $"if(result!=default){{await System.Text.Json.JsonSerializer.SerializeAsync(response.Body,result).ConfigureAwait(false);}}";
            }
        }
    }
}
