# Libra
Libra 是一款基于 Natasha 和 Asp.net Core Web 中间件的弱类型远程调用组件, 得益于 Natasha 强大的动态构建功能,它允许调用方不使用强接口约束,仅通过类及方法名即可完成调用.   

<br> 

## 原理介绍

Libra 允许远程主机通过 **"类名.方法名"** 方式调用本机服务. 还可以通过映射 **Key** => **"类名.方法名"** 对外仅暴露 **Key**.

#### Libra 库类型实例化规则:  

 - 类被 ASP.NET CORE 注入, 则使用服务提供者创建类的实例, 兼容注入功能.
 - 类未被注册 或 来自于插件则使用 **new** 进行实例化.
 - 被调用方法是静态方法, 则直接使用 **静态类** 来调用方法.

#### Libra 库分为请求/分析两部分:  

 - "请求模块" 会序列化 **调用者Key** 及其 **方法参数**, 并将其传递给远程服务的 **/Libra** 中间件来获取数据.
 - "分析模块" 会通过 **调用者Key** 寻找二三优化字典中的委托, 如果未找到委托, 则通过 Natasha 将字符串转换为类型来动态构造委托, 委托入参和返回值均为字符串类型, 该字符串是被包装的方法参数/返回值的序列化结果.   
 
    - 参数包装策略:
      - 当参数仅有1个时, 类型为常规类型: 基元类型/值类型 则被包装到 `LibraSingleParameter<SType>` 中, 以方便序列化.
      - 当参数仅有1个时, 类型为 string: 转换成 utf8-byte 传值.
      - 当参数仅有1个时, 类型为 byte[]: 不参与序列化直接传值.
      - 当参数仅有1个时, 类型为复杂类型: 数组/类/集合/字典 则以当前类型进行序列化.
      - 当参数有多个时, Libra 将包装多个参数到代理类中, 例如 method(string name, int age) 会有对应的代理类 class $uuid { string name ,int age }; 调用时: method( parameter.name, parameter.age);  
      
   - 返回值包装策略:
     - 如果为 void / Task 类型, 返回空.
     - 如果为 string (可能被Task包裹`Task<string>`) 类型, 则直接写入响应函数.
     - 如果为 byte[] (可能被Task包裹`Task<byte[]>`) 类型, 则直接返回结果.
     - 如果为 Stream (可能被Task包裹`Task<Stream>`) 类型, 则直接用流写入结果.
     - 如果为 基元类型/值类型/string (可能被Task包裹), 则被包装到 LibraResult 中序列化返回.
     - 如果为 复杂类型 (可能被Task包裹), 则直接将其序列化返回.  
      

#### Libra 对异步方法的支持:

 - 如果目标方法为 `async Task<X> Method` 形式, 则 Libra 将在包装方法内部使用 `await Method` 异步调用获取结果. 
 
#### Libra 的结果:

 - 抛出异常 : 目标方法不允许调用或者不存在.
 - HTTP状态码 : 代表被调用的方法为 void, 视调用的API而定.
 - `LibraResult<S>` : 正常基元/值类型返回结果.  
 - 其他类型 : 正常返回.

<br> 
   
## 使用方法

 - #### Server端

```C#  

//配置服务
services.AddLibraWpc()

   //给 myDomain 域配置过滤器
   .ConfigureFilter("myDomain", (route,req,rsp) => {  return true;  })
   
   //增加 myDomain 域可用的程序集或方法
   .ConfigureLibraDomain
   (
      "myDomain",
      opt => opt
        .AllowAssembly(Assembly.GetEntryAssembly()) //允许该程序集内所有的类型被远程调用
        .CallerMapper("Hello7", "TeacherService.Hello6") //当远程传来 Hello7 时默认路由到 TeacherService.Hello6
   );

 
 
 //添加服务, 使用请求拦截
 app.UseLibraService();
  
```
- #### 插件调用

```C#  

//Libra 允许客户端远程调用服务端加载的插件方法
//服务端加/卸载插件
LibraDomainManagement.LoadPlugin("myDomain",dllFilePath);
LibraDomainManagement.UnloadPlugin("myDomain",dllFilePath);

```


- #### 客户端

```C#

 LibraClientPool.SetGlobalBaseUrl("https://localhost:5001/");
 
// 调用远程类 TeacherService 中 public byte[] HelloX(double value) 方法, 获取流
await "TeacherService.HelloX".WpcParam(12.34).GetResultAsync<byte[]>();

// 调用远程类 TeacherService 中 public int Hello3(double value) 方法
await "TeacherService.Hello3".WpcParam(12.34).GetResultAsync<int>();

// 调用远程类 TeacherService 中 public void Hello8() 方法, 返回 HttpStatusCode
await "TeacherService.Hello8".WpcParam().GetCodeAsync(); 

// 调用远程类 TeacherService 中 public string Hello6(TestModel model) 方法, 其中 TestModel 结构如: class {int[] Indexs ,string Name}
await "TeacherService.Hello6"
 .WpcParam(new { Indexs = new int[] { 1,2,3,4 }, name="abc" })
 .ConfigUrl(url)
 .GetResultAsync<string>(); 

// 调用远程类 TeacherService 中 public int Hello4(double value, DateTime time) 方法
await "TeacherService.Hello4"
 .WpcParam(new { Value = 12.34, time = DateTime.Now })
 .ConfigUrl(url)
 .GetResultAsync<int>();


```

- #### 强类型客户端
```C#
var stu = LibraProxyClient.CreateClient<IStudent>("https://localhost:5001/");
return await stu.GetStudentName(10,"aaaasdsdsd");
```

- #### 其他用法
```C#   
// 无参配置头信息
"TeacherService.MethodName"
 .WpcParam()
 .ConfigRequest(req=>{ req.Headers.Add("key","value"); })
 .ConfigUrl(url)
 .GetCodeAsync(); 

//调用远程服务器指定域的内容,下例域为 "remoteDomainName"
("remoteDomainName", "TeacherService.MethodName")
 .WpcParam()
 .ConfigRequest(req=>{ req.Headers.Add("key","value"); })
 .ConfigUrl(url)
 .GetCodeAsync(); 

//设置超时调用
using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
{
     return await "TeacherService.Hello7"
      .WpcParam()
      .ConfigCancellationToken(cts.Token)
      .GetResultAsync<int>().ConfigureAwait(false);
}
```


- #### 组播

```C#

var multicast = LibraMulticastHostManagement.GetOrCreate("测试组");
multicast.AppendHost(
   ("https://localhost:5001/", null), 
   ("https://localhost:5001/", req=>{ req.Headers.Add("key","value");})
);
multicast.AppendHost("https://localhost:5001/", req=>{ req.Headers.Add("key","value"); });
multicast.AppendHost("https://localhost:7001/");
multicast.Save();

//返回数组结果
await "TeacherService.Helloxxx".WpcParam().MulticastArrayResultAsync<int>("测试组"); //[ 1, 2]

//返回 LibraMulticastResult 数组, 包括调用的 URL 和 其结果 Result;
await "TeacherService.Helloxxx".WpcParam().MulticastTupleResultAsync<int>("测试组"); //[("https://localhost:5001/",1) ,("https://localhost:7001/",2)]

//远程通知目标主机的 void xxx() 方法, 遇到第一个结果不是 200 / 204 的就返回 false .
await "TeacherService.Helloxxx".WpcParam().MulticastNotifyAsync("测试组"); 

//远程通知目标主机的 bool xxx() 方法, 遇到第一个结果为 false 的就返回 false .
await "TeacherService.Helloxxx".WpcParam().MulticastNotifyAsync<bool>("测试组"); 
```  

<br> 

## 性能优化

感谢 WebApiClient 作者的帮助, 客户端和服务端的序列化直连了 Request 和 Reponse 流操作, 性能得到了提升; 除此之外,在资源复用方面也得到了老九的帮助, 由此, Libra 客户端的发送单元, 避免了多次创建实例与多余的数据填充, 我们绕过了 HttpClient 臃肿的实现, LibraClient 更为轻量级.  

Natasha 支持极复杂的动态构建和编译优化, 我们得以轻松构建高度定制的动态委托来提升性能;  

Natasha 支持创建动态代理, 这让我们的正反序列化均以强类型进行.  

我们使用 DynamicDictionary 作为路由字典, 以便突破并发字典带来的寻址瓶颈.  

<br> 

## 性能计数

#### 单次执行  

 - .netcore3.1 (paintext : 3ms).  
 
 - .net5 (paintext : 1ms).

<br> 

## 计划

 - [ ] 完善各种类型的测试,增加 UT 测试.
 - [x] 增加安全认证.
 - [ ] 接入其他高性能序列化.
 - [x] 实现组播配置.
 - [x] 对接 Request 和 Response 流操作.
 - [x] 增加强约束用法.
 - [ ] 测试网关扩容调用.
 - [ ] 兼容服务发现组件完成高可用调用.
 - [ ] 对接配置中心

还在更新中...
