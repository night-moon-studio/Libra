using System;

namespace Libra.Server.Protocal
{
    public static class LibraReadHandler
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
    }
}
