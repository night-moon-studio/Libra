using System;
using System.Threading.Tasks;

namespace Libra.Server.Protocal
{
    public static class LibraServerProtocal
    {

        public const string DeserializeScript = "Libra.LibraProxyCreator.Deserialize";
        public const string DirectlyScript = "Libra.LibraProxyCreator.GetBytesFromRequest";
        /// <summary>
        /// 获取单个参数时,需要反序列化的参数类型脚本
        /// </summary>
        /// <param name="type">参数类型</param>
        /// <returns></returns>
        public static string GetSingleParameterDeserializeTypeScript(Type parameterType, string parameterName, out string parameterCaller)
        {
            parameterCaller = parameterName;
            if (parameterType.IsPrimitive || parameterType.IsValueType || parameterType == typeof(string))
            {

                //复用这个变量, 此时记录参数的调用逻辑 
                parameterCaller += ".Value";
                //如果是基元类型或者是值类型
                //生成以下逻辑:
                //var parameters = JsonSerializer.Deserialize<LibraSingleParameter<int>>(arg, LibraProtocalAnalysis.JsonOption);
                return $"var {parameterName} = {DeserializeScript}<LibraSingleParameter<{parameterType.GetDevelopName()}>>(request);";
                
            }
            else if (parameterType == typeof(byte[]))
            {
                //如果是byte数组
                //复用这个变量, 此时记录参数的调用逻辑 
                //无需创建临时变量直接从 Request 中获取
                return $"var {parameterName} = {DirectlyScript}(request);";
            }
            else
            {
                //如果是其他类型
                //var parameters = JsonSerializer.Deserialize<ParameterType>(arg, LibraProtocalAnalysis.JsonOption);
                return $"var {parameterName} = {DeserializeScript}<{parameterType.GetDevelopName()}>(request);";
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
            else if (returnType.IsPrimitive || returnType.IsValueType || returnType == typeof(string))
            {

                //如果返回值为妓院类型或者值类型
                //生成执行逻辑代码:
                // var result =  new LibraResult<int>(){ Value = [await] (new TestService()).Hello(parameters.Name,parameters.Age)[.ConfigureAwait(false)] };
                // await JsonSerializer.SerializeAsync((response.Body,result);
                string result = $"var result = new LibraResult<{returnType.GetDevelopName()}>(){{ Value = {(isAsync ? "await" : "")} {methodCaller}{(isAsync ? ".ConfigureAwait(false)" : "")}}};";
                return result + $"await System.Text.Json.JsonSerializer.SerializeAsync(response.Body,result);";
                 

            }
            else if (returnType == typeof(byte[]))
            {

                //如果是byte[]类型,则直接返回
                //生成执行逻辑代码:
                //await response.BodyWriter.WriteAsync([await] (new TestService()).Hello(parameters.Name,parameters.Age)[.ConfigureAwait(false)]);
                //var result = $"var result ={(isAsync ? "await " : "")} {methodCaller}{(isAsync ? ".ConfigureAwait(false)" : "")};";
                return $"await response.BodyWriter.WriteAsync({(isAsync ? "await " : "")} {methodCaller}{(isAsync ? ".ConfigureAwait(false)" : "")});";
                //return result + $"if(result!=null){{ await response.BodyWriter.WriteAsync(result); }}";

            }
            else
            {

                //如果是其他墙类型
                //生成执行逻辑代码:
                // var result = [await] (new TestService()).Hello(parameters.Name,parameters.Age)[.ConfigureAwait(false)];
                // await JsonSerializer.SerializeAsync(response.Body,result);
                var result = $"var result = {(isAsync ? "await" : "")} {methodCaller}{(isAsync ? ".ConfigureAwait(false)" : "")};";
                return result + $"await System.Text.Json.JsonSerializer.SerializeAsync(response.Body,result);";
            }
        }

    }
}
