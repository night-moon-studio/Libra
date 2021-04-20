using Libra.Reciver;
using Natasha.CSharp;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Libra
{
    public static class LibraProtocalAnalysis
    {

        public static JsonSerializerOptions JsonOption;
        public static IServiceProvider Provider;
        private static DynamicDictionaryBase<string, Func<string, string>> _invokeFastCache;
        private static readonly ConcurrentDictionary<string, Func<string, string>> _invokerMapping;
        static LibraProtocalAnalysis()
        {
            JsonOption = new JsonSerializerOptions();
            _invokerMapping = new ConcurrentDictionary<string, Func<string, string>>();
            _invokeFastCache = _invokerMapping.PrecisioTree();
        }

        public static string Call(string caller, string parameters)
        {

            if (_invokeFastCache.TryGetValue(caller, out var func))
            {
                return func(parameters);
            }
            else
            {
                var realType = LibraTypeManagement.GetTypeFromMapper(caller);
                var index = realType.LastIndexOf('.');
                var type = realType.Substring(0, index);
                var method = realType.Substring(index + 1, realType.Length - index - 1);
                try
                {
                    var dynamicFunc = NDelegate
                   .RandomDomain(item => item.LogSyntaxError().LogCompilerError())
                   .Func<Func<string, string>>($"return LibraProtocalAnalysis.HandlerType(\"{caller}\",typeof({type}),\"{method}\");")();
                    if (dynamicFunc != null)
                    {
                        return dynamicFunc(parameters);
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"请核对您所访问的类: {type} 及方法 {method} 是否存在! 额外信息:{ex.Message}");
                }
                return _invokeFastCache[caller](parameters);
            }
           

        }

        public static Func<string,string> HandlerType(string key, Type type, string method)
        {

            if (!LibraTypeManagement.HasMethod(type,method))
            {
                return null;
            }

            var methodInfo = type.GetMethod(method);
            var methodCallBuilder = new StringBuilder();
            var parameterInfos = methodInfo.GetParameters();
            ParameterInfo firstParameterInfo = default;
            string parameterName = default;

            var classBuilder = new StringBuilder();
            if (parameterInfos.Length > 1)
            {

                parameterName = "parameters";
                var parameterBuilder = new StringBuilder();
                var className = "N"+Guid.NewGuid().ToString("N");
                classBuilder.Append($"public class {className}{{");
                foreach (var item in parameterInfos.OrderBy(c=>c.Position))
                {
                    parameterBuilder.Append($"parameters.{item.Name},");
                    classBuilder.Append($"public {item.ParameterType.GetDevelopName()} {item.Name} {{ get;set; }}");
                }
                classBuilder.Append('}');
                methodCallBuilder.AppendLine($"var {parameterName} = string.IsNullOrEmpty(arg) ? default : System.Text.Json.JsonSerializer.Deserialize<{className}>(arg,LibraProtocalAnalysis.JsonOption);");
                
                parameterBuilder.Length -= 1;
                parameterName = parameterBuilder.ToString();

            }
            else if (parameterInfos.Length == 1)
            {
                parameterName = "parameters";
                firstParameterInfo = parameterInfos[0];
                var pType = firstParameterInfo.ParameterType;
                if (pType.IsPrimitive || pType == typeof(string) || pType == typeof(DateTime))
                {

                    methodCallBuilder.AppendLine($"var {parameterName} = string.IsNullOrEmpty(arg) ? default : System.Text.Json.JsonSerializer.Deserialize<LibraSingleParameter<{firstParameterInfo.ParameterType.GetDevelopName()}>>(arg,LibraProtocalAnalysis.JsonOption);");
                    parameterName += ".Value";

                }
                else
                {
                    methodCallBuilder.AppendLine($"var {parameterName} = string.IsNullOrEmpty(arg) ? default : System.Text.Json.JsonSerializer.Deserialize<{firstParameterInfo.ParameterType.GetDevelopName()}>(arg,LibraProtocalAnalysis.JsonOption);");
                }

            }


            //获取调用者
            string caller = default;
            if (methodInfo.IsStatic)
            {
                caller = type.GetDevelopName();
            }
            else
            {
                caller = $"LibraProtocalAnalysis.Provider.GetService<{type.GetDevelopName()}>()";
            }


            //调用
            if (methodInfo.ReturnType != typeof(void))
            {
                methodCallBuilder.AppendLine($"var result = new LibraResult<{methodInfo.ReturnType.GetDevelopName()}>(){{ Value = {caller}.{methodInfo.Name}({parameterName}) }};");
                methodCallBuilder.AppendLine($"return System.Text.Json.JsonSerializer.Serialize(result);");
            }
            else
            {
                methodCallBuilder.AppendLine($"{caller}.{methodInfo.Name}({parameterName});");
                methodCallBuilder.AppendLine("return \"\";");
            }


            var func = NDelegate
                .RandomDomain(item =>
                {
                    item
                    .LogSyntaxError()
                    .UseFileCompile();
                })
                
                .SetClass(item => item.AllowPrivate(type).Body(classBuilder.ToString()))
                .Func<string, string>(methodCallBuilder.ToString());
            _invokerMapping[key] = func;
            _invokeFastCache = _invokerMapping.PrecisioTree();
            return func;
        }

    }

}
