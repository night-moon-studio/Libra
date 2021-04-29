using Libra.Server.Protocal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Natasha.CSharp;
using Natasha.CSharp.Reverser;
using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace Libra
{

    public delegate Task ExecuteLibraMethod(HttpRequest request, HttpResponse response); 
    /// <summary>
    /// Libra 协议分析及执行类
    /// </summary>
    public static class LibraProxyCreator
    {
        public static IServiceCollection DIService;
        public static JsonSerializerOptions JsonOption;
        public static IServiceProvider Provider;
        
        static LibraProxyCreator()
        {
            JsonOption = new JsonSerializerOptions();
        }


        /// <summary>
        /// 反序列化实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">HTTP请求</param>
        /// <returns></returns>
        public static T Deserialize<T>(HttpRequest request)
        {
            
            request.EnableBuffering();
            var bufferResult = request.BodyReader.ReadAsync().Result;
            if (bufferResult.Buffer.IsEmpty)
            {
                return default(T);
            }
            var reader = new Utf8JsonReader(bufferResult.Buffer);
            return JsonSerializer.Deserialize<T>(ref reader, JsonOption);

        }

        /// <summary>
        /// 直接获取bytes
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static byte[] GetBytesFromRequest(HttpRequest request)
        {
            request.EnableBuffering();
            var bufferResult = request.BodyReader.ReadAsync().Result;
            if (bufferResult.Buffer.IsEmpty)
            {
                return null;
            }
            return bufferResult.Buffer.ToArray();
        }


        /// <summary>
        /// 创建执行委托
        /// </summary>
        /// <param name="route">路由</param>
        /// <param name="response">回应实体</param>
        /// <returns></returns>
        public static async Task<ExecuteLibraMethod> CreateDelegate(string route, HttpResponse response)
        {
            //检查是否为映射类型,如果是则获取真实的 "类名.方法名"
            var realType = LibraTypeManagement.GetTypeFromMapper(route);

            //获取类名及方法名
            var index = realType.LastIndexOf('.');
            var typeName = realType.AsSpan().Slice(0, index).ToString();
            var methodName = realType.AsSpan().Slice(index + 1, realType.Length - index - 1).ToString();
            try
            {

                //从插件管理获取方法所在的域
                var domain = LibraPluginManagement.GetPluginDominByType(typeName);
                NDelegate nDelegate = default;
                if (domain != null)
                {
                    nDelegate = NDelegate.UseDomain(domain);
                }
                else
                {
                    nDelegate = NDelegate.RandomDomain();
                }
                //将类型字符串转换成运行时类型传参生成调用委托
                var dynamicFunc = nDelegate
                        .Func<ExecuteLibraMethod>($"return LibraProxyCreator.CreateDelegate(typeof({typeName}),\"{methodName}\",\"{typeName}\");")();


                if (dynamicFunc != null)
                {
                    //如果生成委托,如果是插件委托,则记录到管理类中
                    if (domain != null)
                    {
                        LibraPluginManagement.AddRecoder(domain, route);
                    }
                    //执行委托返回结果
                    return dynamicFunc;
                }
                else
                {
                    //如果未成功生成委托,则说明该调用不符合系统规范,可能不存在
                    response.StatusCode = 404;
                    await response.WriteAsync($"请核对您所访问的类: {typeName} 及方法 {methodName} 是否存在!");

                }

            }
            catch (Exception ex)
            {

                response.StatusCode = 501;
                await response.WriteAsync($"创建: {typeName}.{methodName} 时出错! 额外信息:{ex.Message}");

            }
            return null;
        }


        /// <summary>
        /// 生成动态委托
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static ExecuteLibraMethod CreateDelegate(Type type, string methodName, string typeName)
        {
            //NSucceedLog.Enabled = true;
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
                methodCallBuilder.AppendLine($"var {parameterName} = {LibraServerProtocal.DeserializeScript}<{className}>(request);");
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
                methodCallBuilder.AppendLine(LibraServerProtocal.GetSingleParameterDeserializeTypeScript(pType, parameterName, out string parameterCaller));
                parameterName = parameterCaller;

            }

            Provider = DIService.BuildServiceProvider();
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
                    caller = $"LibraProxyCreator.Provider.GetService<{type.GetDevelopName()}>()";

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

            methodCallBuilder.AppendLine(LibraServerProtocal.GetReturnScript(returnType, $"{caller}.{methodInfo.Name}({parameterName})", isAsync));
            
            //使用 Natasha 进行动态方法构造
            var delegateFunc = NDelegate
                .UseDomain(domain, item =>
                {
                    item
                    .ThrowCompilerError()    //开启语法错误日志
                    .ThrowSyntaxError(); //开启编译错误日志
                })
                .SetClass(item => item.AllowPrivate(type).Body(classBuilder.ToString())); //将代理类添加到当前构造类的Body中去


            ExecuteLibraMethod func;
            //如果是异步方法,需要 async Task 来执行 await, 构造出异步方法
            func = delegateFunc.AsyncDelegate<ExecuteLibraMethod>(methodCallBuilder.ToString());
            return func;
        }

    }

}
