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
    /// <summary>
    /// Libra 协议分析及执行类
    /// </summary>
    public static class LibraProtocalAnalysis
    {

        public static JsonSerializerOptions JsonOption;
        public static IServiceProvider Provider;
        private static DynamicDictionaryBase<string, Func<byte[], byte[]>> _invokeFastCache;
        private static readonly ConcurrentDictionary<string, Func<byte[], byte[]>> _invokerMapping;
        static LibraProtocalAnalysis()
        {
            JsonOption = new JsonSerializerOptions();
            _invokerMapping = new ConcurrentDictionary<string, Func<byte[], byte[]>>();
            _invokeFastCache = _invokerMapping.PrecisioTree();
        }

        /// <summary>
        /// 批量移除已缓存的方法映射
        /// </summary>
        /// <param name="keys"></param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Remove(IEnumerable<string> keys)
        {

            Func<byte[], byte[]> func = null;
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

        /// <summary>
        /// 异步执行
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="parameters"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public static async Task<byte[]> CallAsync(string caller, byte[] parameters, HttpResponse response)
        {
            
            if (_invokeFastCache.TryGetValue(caller, out var func))
            {
                return func(parameters);
            }
            else
            {
                //检查是否为映射类型,如果是则获取真实的 "类名.方法名"
                var realType = LibraTypeManagement.GetTypeFromMapper(caller);

                //获取类名及方法名
                var index = realType.LastIndexOf('.');
                var typeName = realType.Substring(0, index);
                var methodName = realType.Substring(index + 1, realType.Length - index - 1);
                try
                {

                    //从插件管理获取方法所在的域
                    var domain = LibraPluginManagement.GetPluginDominByType(typeName);
                    NDelegate nDelegate = default;
                    if (domain!=null)
                    {
                        nDelegate = NDelegate.UseDomain(domain);
                    }
                    else
                    {
                        nDelegate = NDelegate.RandomDomain();
                    }
                    //将类型字符串转换成运行时类型传参生成调用委托
                    var dynamicFunc = nDelegate
                            .Func<Func<byte[], byte[]>>($"return LibraProtocalAnalysis.CreateDelegate(\"{caller}\",typeof({typeName}),\"{methodName}\",\"{typeName}\");")();
                    
                    
                    if (dynamicFunc != null)
                    {
                        //如果生成委托,如果是插件委托,则记录到管理类中
                        if (domain != null)
                        {
                            LibraPluginManagement.AddRecoder(domain, caller);
                        }
                        //执行委托返回结果
                        return dynamicFunc(parameters);
                    }
                    else
                    {
                        //如果未成功生成委托,则说明该调用不符合系统规范,可能不存在
                        response.StatusCode = 404;
                        await response.WriteAsync($"请核对您所访问的类: {typeName} 及方法 {methodName} 是否存在!");
                        return null;
                    }

                }
                catch (Exception ex)
                {

                    response.StatusCode = 404;
                    await response.WriteAsync($"请核对您所访问的类: {typeName} 及方法 {methodName} 是否存在! 额外信息:{ex.Message}");
                    return null;

                }
            }
           

        }

        /// <summary>
        /// 生成动态委托
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Func<byte[],byte[]> CreateDelegate(string key, Type type, string methodName, string typeName)
        {

            //判断类型是否来自插件,如果是则获取插件域
            var isPlugin = false;
            var domain = LibraPluginManagement.GetPluginDominByType(typeName);

            //如果不属于插件委托并且记录中没有该类型的映射,则说明该调用不被允许
            if (!LibraTypeManagement.HasMethod(type,methodName) && domain == null)
            {
                return null;
            }

            //如果不属于插件域则赋值一个随即域
            if (domain == null)
            {
                domain = DomainManagement.Random;
            }
            else
            {
                isPlugin = true;
            }


            //获取方法元数据
            var methodInfo = type.GetMethod(methodName);
            //方法代码
            var methodCallBuilder = new StringBuilder();
            var classBuilder = new StringBuilder();


            var parameterInfos = methodInfo.GetParameters();
            ParameterInfo firstParameterInfo = default;
            string parameterName = default;

           
            if (parameterInfos.Length > 1)
            {
                //大于1个参数时
                //参数临时变量名未 parameters
                parameterName = "parameters";

                //传参字符串构建
                var parameterBuilder = new StringBuilder();

                //多个参数需要由代理类,创建代理类
                var className = "N"+Guid.NewGuid().ToString("N");
                classBuilder.Append($"public class {className}{{");
                foreach (var item in parameterInfos.OrderBy(c=>c.Position))
                {
                    //创建 parameters 参数调用代码如: method(parameters.Age,parameters.Name)
                    parameterBuilder.Append($"parameters.{item.Name},");
                    //创建类的属性,如: public class ProxyClass{ public string Name{get;set;} public int Age{get;set;} }
                    classBuilder.Append($"public {item.ParameterType.GetDevelopName()} {item.Name} {{ get;set; }}");

                }
                classBuilder.Append('}');

                //参数赋值逻辑代码:
                //if(arg == null){
                //  parameters = default;
                //}
                //else{
                //  parameters = JsonSerializer.Deserialize<ProxyClass>(arg, LibraProtocalAnalysis.JsonOption);
                //}
                methodCallBuilder.AppendLine($"var {parameterName} = arg == null ? default : System.Text.Json.JsonSerializer.Deserialize<{className}>(arg,LibraProtocalAnalysis.JsonOption);");
                
                //移除最后一个都好
                parameterBuilder.Length -= 1;
                //复用这个变量,临时变量名后面无需再用,后可作为参数逻辑使用.
                parameterName = parameterBuilder.ToString();

            }
            else if (parameterInfos.Length == 1)
            {
                //当仅由一个参数时
                //临时变量名为 parameters
                parameterName = "parameters";
                //获取这个参数
                firstParameterInfo = parameterInfos[0];
                //获取参数类型
                var pType = firstParameterInfo.ParameterType;

                if (pType.IsPrimitive ||  pType.IsValueType)
                {

                    //如果是基元类型或者是值类型
                    //生成以下逻辑:
                    //if(arg == null){
                    //  parameters = default;
                    //}
                    //else{
                    //  parameters = JsonSerializer.Deserialize<LibraSingleParameter<int>>(arg, LibraProtocalAnalysis.JsonOption);
                    //}
                    methodCallBuilder.AppendLine($"var {parameterName} = arg == null ? default : System.Text.Json.JsonSerializer.Deserialize<LibraSingleParameter<{firstParameterInfo.ParameterType.GetDevelopName()}>>(arg,LibraProtocalAnalysis.JsonOption);");
                    //复用这个变量, 此时记录参数的调用逻辑 
                    //parameters.Value
                    parameterName += ".Value";

                }
                else if (pType == typeof(byte[]))
                {
                    //如果是byte数组
                    //复用这个变量, 此时记录参数的调用逻辑 
                    //无需创建临时变量直接使用函数参数即可
                    parameterName = "arg";
                }
                else
                {
                    //如果是其他类型
                    //if(arg == null){
                    //  parameters = default;
                    //}
                    //else{
                    //  parameters = JsonSerializer.Deserialize<ParameterType>(arg, LibraProtocalAnalysis.JsonOption);
                    //}
                    methodCallBuilder.AppendLine($"var {parameterName} = arg == null ? default : System.Text.Json.JsonSerializer.Deserialize<{firstParameterInfo.ParameterType.GetDevelopName()}>(arg,LibraProtocalAnalysis.JsonOption);");
                }

            }


            //获取调用者
            string caller = default;
            if (methodInfo.IsStatic)
            {

                //如果是静态方法,则直接使用类名进行调用
                caller = type.GetDevelopName();
            }
            else
            {

                //不是静态方法
                if (isPlugin)
                {

                    //如果是插件方法,则直接 new, 如: (new TestServiceFromPlugin())
                    caller = $"(new {type.GetDevelopName()}())";

                }
                else if (Provider.GetService(type) == null)
                {

                    //检测该类型是否被注入,不是则直接 new. 如 (new TestService())
                    caller = $"(new {type.GetDevelopName()}())";

                }
                else
                {

                    //如果该类型被注入了,则使用 asp.net core的 provider 进行创建
                    caller = $"LibraProtocalAnalysis.Provider.GetService<{type.GetDevelopName()}>()";

                }
               
            }


            //结果处理
            bool isAsync = AsyncReverser.GetAsync(methodInfo) != null;
            Type returnType = methodInfo.ReturnType;
            if (isAsync)
            {
                //如果是异步返回值
                if (returnType!=typeof(Task))
                {
                    //获取到被Task<T> 包裹的类型,如: T
                    returnType = methodInfo.ReturnType.GenericTypeArguments[0];
                }
                
            }


            if (returnType == typeof(void) || returnType == typeof(Task))
            {

                //如果返回值为 void 或者是 Task
                //生成执行逻辑代码:
                // (new TestService()).Hello(parameters.Name,parameters.Age);
                // 或
                // await (new TestService()).Hello(parameters.Name,parameters.Age).ConfigureAwait(false);
                methodCallBuilder.AppendLine($"{(isAsync ? "await" : "")} {caller}.{methodInfo.Name}({parameterName}){(isAsync ? ".ConfigureAwait(false)" : "")};");
                methodCallBuilder.AppendLine("return null;");

            }
            else if(returnType.IsPrimitive || returnType.IsValueType)
            {

                //如果返回值为妓院类型或者值类型
                //生成执行逻辑代码:
                // var result =  new LibraResult<int>(){ Value = (new TestService()).Hello(parameters.Name,parameters.Age) };
                // return JsonSerializer.SerializeToUtf8Bytes(result);
                // 或
                // var result =  new LibraResult<int>(){ Value = await (new TestService()).Hello(parameters.Name,parameters.Age).ConfigureAwait(false) };
                // return JsonSerializer.SerializeToUtf8Bytes(result);
                methodCallBuilder.AppendLine($"var result = new LibraResult<{returnType.GetDevelopName()}>(){{ Value = {(isAsync ? "await" : "")} {caller}.{methodInfo.Name}({parameterName}){(isAsync ? ".ConfigureAwait(false)" : "")}}};");
                methodCallBuilder.AppendLine($"return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(result);");

            }
            else if (returnType == typeof(byte[]))
            {

                //如果是byte[]类型,则直接返回
                //生成执行逻辑代码:
                //return  (new TestService()).Hello(parameters.Name,parameters.Age);
                //或
                //return await (new TestService()).Hello(parameters.Name,parameters.Age).ConfigureAwait(false);
                methodCallBuilder.AppendLine($"return {(isAsync ? "await" : "")} {caller}.{methodInfo.Name}({parameterName}){(isAsync ? ".ConfigureAwait(false)" : "")};");

            }
            else
            {

                //如果是其他墙类型
                //生成执行逻辑代码:
                // var result = (new TestService()).Hello(parameters.Name,parameters.Age);
                // return JsonSerializer.SerializeToUtf8Bytes(result);
                // 或
                // var result = await (new TestService()).Hello(parameters.Name,parameters.Age).ConfigureAwait(false);
                // return JsonSerializer.SerializeToUtf8Bytes(result);
                methodCallBuilder.AppendLine($"var result = {(isAsync ? "await" : "")} {caller}.{methodInfo.Name}({parameterName}){(isAsync ? ".ConfigureAwait(false)" : "")};");
                methodCallBuilder.AppendLine($"return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(result);");
            }
            

            //使用 Natasha 进行动态方法构造
            var delegateFunc = NDelegate
                .UseDomain(domain, item =>
                {
                    item
                    .LogSyntaxError()    //开启语法错误日志
                    .LogCompilerError(); //开启编译错误日志
                })
                .SetClass(item => item.AllowPrivate(type).Body(classBuilder.ToString())); //将代理类添加到当前构造类的Body中去


            Func<byte[], byte[]> func;
            if (isAsync)
            {
                //如果是异步方法,需要 async Task 来执行 await, 构造出异步方法
                var tempFunc = delegateFunc.AsyncFunc<byte[], Task<byte[]>>(methodCallBuilder.ToString());
                //这里包裹一层,直接返回上面异步方法的结果
                func = (param) => tempFunc(param).Result;
            }
            else
            {
                //动态构造方法
                func = delegateFunc.Func<byte[], byte[]>(methodCallBuilder.ToString());
            }


            //添加到字典
            _invokerMapping[key] = func;
            //从字典转换到精确快速查找树
            _invokeFastCache = _invokerMapping.PrecisioTree();
            return func;
        }

    }

}
