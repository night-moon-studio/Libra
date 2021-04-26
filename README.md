# Libra
Libra 是一款基于 Natasha 和 Asp.net Core Web 中间件的远程调用组件, 得益于 Natasha 强大的动态构建功能,它允许调用方不使用强接口约束,仅通过类及方法名即可完成调用.   

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
      - 当参数仅有1个时, 类型为常规类型: 基元类型/值类型/string 则被包装到 `LibraSingleParameter<SType>` 中, 以方便序列化.
      - 当参数仅有1个时, 类型为 byte[]: 不参与序列化直接传值.
      - 当参数仅有1个时, 类型为复杂类型: 数组/类/集合/字典 则以当前类型进行序列化.
      - 当参数有多个时, Libra 将包装多个参数到代理类中, 例如 method(string name, int age) 会有对应的代理类 class $uuid { string name ,int age }; 调用时: method( parameter.name, parameter.age);  
      
   - 返回值包装策略:
     - 如果为 void / Task 类型, 返回空.
     - 如果为 byte[] (可能被Task包裹`Task<byte[]>`) 类型, 则直接返回结果.
     - 如果为 基元类型/值类型/string (可能被Task包裹), 则被包装到 LibraResult 中序列化返回.
     - 如果为 复杂类型 (可能被Task包裹), 则直接将其序列化返回.  
      

#### Libra 对异步方法的支持:

 - 如果目标方法为 `async Task<X> Method` 形式, 则 Libra 将在包装方法内部使用 `await Method` 异步调用获取结果. 
 
#### Libra 的结果:

 - 抛出异常 : 则目标方法不允许调用或者不存在.
 - HTTP状态码 : 则代表被调用的方法为 void, 视调用的API而定.
 - byte[] : 如果目标方法返回 byte[] 则为目标方法的执行结果, 否则为 Reponse.Body 中读取的流.
 - `LibraResult<S>` : 正常基元/值/string类型返回结果.  
 - 类实例 : 复杂类型返回结果.

<br> 

## 功能列表

1. 配置允许被远程调用的类型
   - 允许调用本机某程序集内的某类的某方法. 
   - 允许调用本机某程序集内带有指定接口约束的类及其方法.
   - 允许以键值对方式配置映射.

2. 配置默认远程地址
   - 当 API 中不传地址时则使用默认地址.
   - 若向 API 中传递 URL 时需要 URL+"/Libra"

3. 配置 Libra 传输使用的 Json 选项.
  
4. 配置组播实现整组调用.
  
<br> 
   
## 使用方法

 - #### Server端

```C#  

//配置服务
services.AddLibraWpc()
   .ConfigureJson( json => { json.PropertyNameCaseInsensitive = true; })
   .ConfigureLibra
   (
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
LibraPluginManagement.LoadPlugin(dllFilePath);
LibraPluginManagement.UnloadPlugin(dllFilePath);

```


- #### 客户端

```C#

// 调用远程类 TeacherService 中 public byte[] HelloX(double value) 方法, 获取流
"TeacherService.HelloX".WpcParam(12.34).GetBytes();

// 调用远程类 TeacherService 中 public int Hello3(double value) 方法
"TeacherService.Hello3".WpcParam(12.34).GetResult<int>();

// 调用远程类 TeacherService 中 public void Hello8() 方法, 返回 HttpStatusCode
"TeacherService.Hello8".NoWpcParam().GetCode(); 

// 调用远程类 TeacherService 中 public string Hello6(TestModel model) 方法, 其中 TestModel 结构如: class {int[] Indexs ,string Name}
"TeacherService.Hello6".WpcParam(new { Indexs = new int[] { 1,2,3,4 }, name="abc" }).GetResult<string>(url); 

// 调用远程类 TeacherService 中 public int Hello4(double value, DateTime time) 方法
"TeacherService.Hello4".WpcParam(new { Value = 12.34, time = DateTime.Now }).GetResult<int>(url);

```

- #### 组播

```C#

var multicast = LibraMulticastHostManagement.GetOrCreate("测试组");
multicast.AppendHost("https://localhost:5001/");
multicast.AppendHost("https://localhost:7001/");

//返回数组结果
"TeacherService.Helloxxx".NoWpcParam().MulticastArrayResult<int>("测试组"); //[ 1, 2]

//返回 LibraMulticastResult 数组, 包括调用的 URL 和 其结果 Result;
"TeacherService.Helloxxx".NoWpcParam().MulticastTupleResult<int>("测试组"); //[("https://localhost:5001/",1) ,("https://localhost:7001/",2)]

//远程通知目标主机的 void xxx() 方法, 遇到第一个结果不是 200 / 204 的就返回 false .
"TeacherService.Helloxxx".NoWpcParam().MulticastNotifyAsync("测试组"); 

//远程通知目标主机的 bool xxx() 方法, 遇到第一个结果为 false 的就返回 false .
"TeacherService.Helloxxx".NoWpcParam().MulticastNotifyAsync<bool>("测试组"); 
```  

<br> 

## 性能优化

感谢 WebApiClient 作者的帮助, 客户端和服务端的序列化直连了 Request 和 Reponse 流操作, 性能得到了提升; 除此之外,在资源复用方面也得到了老九的帮助, 由此, Libra 客户端的发送单元, 避免了多次创建实例与多余的数据填充.
Natasha 支持极复杂的动态构建和编译优化, 我们得以轻松构建高度定制的动态委托来提升性能;
Natasha 支持创建动态代理, 这让我们的正反序列化均以强类型进行.
我们使用 DynamicDictionary 作为路由字典, 以便突破并发字典带来的寻址损耗.


<br> 

## 计划

 - [ ] 完善各种类型的测试,增加 UT 测试.
 - [ ] 增加安全认证.
 - [ ] 接入其他高性能序列化.
 - [x] 实现组播配置.
 - [x] 对接 Request 和 Response 流操作.
 - [ ] 单播/组播的高并发任务合并/转发/执行.

还在更新中...
