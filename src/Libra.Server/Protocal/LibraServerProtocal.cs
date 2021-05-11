using System;
using System.IO;
using System.Threading.Tasks;

namespace Libra.Server.Protocal
{
    public static class LibraServerProtocal
    {

        public const string DeserializeScript = "await Libra.LibraProxyCreator.Deserialize";
        public const string DirectlyScript = "await Libra.LibraProxyCreator.GetBytesFromRequest";
        public const string DeserializeToStringScript = "await Libra.LibraProxyCreator.GetStringFromRequest";
        /// <summary>
        /// 获取单个参数时,需要反序列化的参数类型脚本
        /// </summary>
        /// <param name="type">参数类型</param>
        /// <returns></returns>
        public static string GetSingleParameterDeserializeTypeScript(Type parameterType, string parameterName, out string parameterCaller)
        {
            parameterCaller = parameterName;
            if (parameterType.IsPrimitive || parameterType.IsValueType)
            {

                //复用这个变量, 此时记录参数的调用逻辑 
                parameterCaller += ".Value";
                //如果是基元类型或者是值类型
                //生成以下逻辑:
                //var parameters = JsonSerializer.Deserialize<LibraSingleParameter<int>>(arg, LibraProtocalAnalysis.JsonOption);
                return $"var {parameterName} = {DeserializeScript}<LibraSingleParameter<{parameterType.GetDevelopName()}>>(request).ConfigureAwait(false);";

            }
            else if (parameterType == typeof(string))
            {

                return $"var {parameterName} = {DeserializeToStringScript}(request).ConfigureAwait(false);"; 

            }
            else if (parameterType == typeof(byte[]))
            {

                //如果是byte数组
                //复用这个变量, 此时记录参数的调用逻辑 
                //无需创建临时变量直接从 Request 中获取
                return $"var {parameterName} = {DirectlyScript}(request).ConfigureAwait(false);";

            }
            else
            {

                //如果是其他类型
                //var parameters = JsonSerializer.Deserialize<ParameterType>(arg, LibraProtocalAnalysis.JsonOption);
                return $"var {parameterName} = {DeserializeScript}<{parameterType.GetDevelopName()}>(request).ConfigureAwait(false);";

            }

        }

        /// <summary>
        /// 根据返回值类型,获取返回脚本
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetReturnScript(Type returnType, string methodCaller, bool isAsync)
        {
            if (returnType == typeof(void) || returnType == typeof(Task))
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
