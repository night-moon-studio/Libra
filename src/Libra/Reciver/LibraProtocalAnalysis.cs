using Microsoft.AspNetCore.Http;
using Natasha.CSharp;
using Natasha.CSharp.Reverser;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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


        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Remove(IEnumerable<string> keys)
        {
            Func<string, string> func = null;
            foreach (var item in keys)
            {
                if (_invokerMapping.ContainsKey(item))
                {
                    while (!_invokerMapping.TryRemove(item, out func));
                    
                }
            }
            func?.DisposeDomain();
            _invokeFastCache = _invokerMapping.PrecisioTree();
        }

        public static async Task<string> CallAsync(string caller, string parameters, HttpResponse response)
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
                    var domain = LibraPluginManagement.GetPluginDominByType(type);
                    NDelegate nDelegate = default;
                    if (domain!=null)
                    {
                        nDelegate = NDelegate.UseDomain(domain, item => item.LogSyntaxError().LogCompilerError());
                    }
                    else
                    {
                        nDelegate = NDelegate.RandomDomain(item => item.LogSyntaxError().LogCompilerError());
                    }
                    var dynamicFunc = nDelegate
                   .Func<Func<string, string>>($"return LibraProtocalAnalysis.HandlerType(\"{caller}\",typeof({type}),\"{method}\",\"{type}\");")();
                    if (dynamicFunc != null)
                    {
                        if (domain != null)
                        {
                            LibraPluginManagement.AddRecoder(domain, caller);
                        }
                        return dynamicFunc(parameters);
                    }
                    else
                    {
                        response.StatusCode = 404;
                        await response.WriteAsync($"请核对您所访问的类: {type} 及方法 {method} 是否存在!");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    response.StatusCode = 404;
                    await response.WriteAsync($"请核对您所访问的类: {type} 及方法 {method} 是否存在! 额外信息:{ex.Message}");
                    return null;
                }
            }
           

        }

        public static Func<string,string> HandlerType(string key, Type type, string methodName, string typeName)
        {
            var isPlugin = false;
            var domain = LibraPluginManagement.GetPluginDominByType(typeName);
            if (!LibraTypeManagement.HasMethod(type,methodName) && domain == null)
            {
                return null;
            }
            if (domain == null)
            {
                domain = DomainManagement.Random;
            }
            else
            {
                isPlugin = true;
            }

            var methodInfo = type.GetMethod(methodName);

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
                if (pType.IsPrimitive ||  pType.IsValueType)
                {

                    methodCallBuilder.AppendLine($"var {parameterName} = string.IsNullOrEmpty(arg) ? default : System.Text.Json.JsonSerializer.Deserialize<LibraSingleParameter<{firstParameterInfo.ParameterType.GetDevelopName()}>>(arg,LibraProtocalAnalysis.JsonOption);");
                    parameterName += ".Value";

                }
                else if(pType == typeof(string))
                {
                    methodCallBuilder.AppendLine($"var {parameterName} = arg;");
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
                //这里不能用 Provider 检测, 否则不能卸载
                if (isPlugin)
                {
                    caller = $"(new {type.GetDevelopName()}())";
                }
                else if (Provider.GetService(type) == null)
                {
                    caller = $"(new {type.GetDevelopName()}())";
                }
                else
                {
                    caller = $"LibraProtocalAnalysis.Provider.GetService<{type.GetDevelopName()}>()";
                }
               
            }

            bool isAsync = AsyncReverser.GetAsync(methodInfo) != null;
            Type returnType = methodInfo.ReturnType;
            if (isAsync)
            {
                if (returnType!=typeof(Task))
                {
                    returnType = methodInfo.ReturnType.GenericTypeArguments[0];
                }
                
            }
            //调用
            if (returnType == typeof(void) || returnType == typeof(Task))
            {
                methodCallBuilder.AppendLine($"{(isAsync ? "await" : "")} {caller}.{methodInfo.Name}({parameterName}){(isAsync ? ".ConfigureAwait(false)" : "")};");
                methodCallBuilder.AppendLine("return \"\";");
            }
            else if (returnType == typeof(string))
            {
                methodCallBuilder.AppendLine($"return {(isAsync ? "await" : "")} {caller}.{methodInfo.Name}({parameterName}){(isAsync ? ".ConfigureAwait(false)" : "")};");
            }
            else if(returnType.IsPrimitive || returnType.IsValueType)
            {
                methodCallBuilder.AppendLine($"var result = new LibraResult<{returnType.GetDevelopName()}>(){{ Value = {(isAsync ? "await" : "")} {caller}.{methodInfo.Name}({parameterName}){(isAsync ? ".ConfigureAwait(false)" : "")}}};");
                methodCallBuilder.AppendLine($"return System.Text.Json.JsonSerializer.Serialize(result);");
            }
            else
            {
                methodCallBuilder.AppendLine($"var result = {(isAsync ? "await" : "")} {caller}.{methodInfo.Name}({parameterName}){(isAsync ? ".ConfigureAwait(false)" : "")};");
                methodCallBuilder.AppendLine($"return System.Text.Json.JsonSerializer.Serialize(result);");
            }

            var delegateFunc = NDelegate.UseDomain(domain, item =>
            {
                item
                .LogSyntaxError()
                .UseFileCompile();
            })
                .SetClass(item => item.AllowPrivate(type).Body(classBuilder.ToString()));
            Func<string, string> func;
            if (isAsync)
            {
                var tempFunc = delegateFunc.AsyncFunc<string, Task<string>>(methodCallBuilder.ToString());
                func = (param) => tempFunc(param).Result;
            }
            else
            {
                func = delegateFunc.Func<string, string>(methodCallBuilder.ToString());
            }
            _invokerMapping[key] = func;
            _invokeFastCache = _invokerMapping.PrecisioTree();
            return func;
        }

    }

}
