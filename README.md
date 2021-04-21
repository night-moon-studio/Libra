# Libra
Libra 是一款基于 Natasha 的 Web 远程调用组件, 得益于 Natasha 强大的动态构建功能,它允许调用方不使用强接口约束,仅通过类及方法名即可完成调用. 

## 原理介绍

Libra 允许远程主机通过 **"类名.方法名"** 方式调用本机服务. 还可以通过映射 **Key** => **"类名.方法名"** 对外仅暴露 **Key**. Libra 对类的实例化会优先寻找该类是否被依赖注入, 如果注入则使用 ASP.NET Core 框架中的服务提供者创建类的实例, 未注入则使用 **new** 进行实例化, 如果被调用方法是静态方法, 则直接使用**静态类**来调用方法.

Libra 库分为请求/分析两部分:  

 - "请求模块" 会序列化 **调用者Key** 及其 **方法参数**, 并将其传递给远程服务的 **/Libra** 路由接口来获取数据.
 - "分析模块" 会通过 **调用者Key** 寻找二三优化字典中的委托, 如果未找到委托, 则通过 Natasha 将字符串转换为类型来动态构造委托, 委托入参和返回值均为字符串类型, 该字符串是被包装的方法参数/返回值的序列化结果.   
 
    - 参数包装策略:
      - 当参数仅有1个时, 类型为常规类型: 基元类型\string\DateTime 则被包装到 LibraSingleParameter<SType> 中, 以方便序列化.
      - 当参数仅有1个时, 类型为复杂类型: 数组\类\集合\字典 则以当前类型进行序列化.
      - 当参数有多个时, Libra 将包装多个参数到代理类中, 例如 method(string name, int age) 会有对应的代理类 class $uuid { string name ,int age }; 调用时: method( parameter.name, parameter.age);  
      
    - 返回值包装策略:
      - 如果为 void 类型, 返回空.
      - 如果为其他类型, 则包装到 LibraResult<RType> 中序列化返回.


## 功能列表

1. 配置允许被远程调用的类型
   - 允许调用本机某程序集内的某类的某方法. 
   - 允许调用本机某程序集内带有指定接口约束的类及其方法.
   - 允许以键值对方式配置映射.

2. 配置默认远程地址
   - 当 API 中不传地址时则使用默认地址.

3. 配置 Libra 传输使用的 Json 选项

## 使用方法


#### Server端

```C#
 services
  .AddLibraJson(json => { json.PropertyNameCaseInsensitive = true; });
  .AddLibraWpc(opt => opt
          //允许该程序集内所有的类型被远程调用
         .AllowAssembly(Assembly.GetEntryAssembly()) 
         //使用实现了 IGetHello 接口的类
         .AllowAssembly<IGetHello>(Assembly.Load("PluginService"))
         //当远程传来 Hello7 时默认路由到 TeacherService.Hello6
         .FlagMapper("Hello7", "TeacherService.Hello6") 
 ); 
  
```
#### 插件调用

```C#
//Libra 允许客户端远程调用服务端加载的插件方法
LibraPluginManagement.AddPlugin(filePath);
LibraPluginManagement.Dispose(filePath);

```

### 客户端

```C#

// 调用远程类 TeacherService 中 public int Hello3(double value) 方法
"TeacherService.Hello3".WpcParam(12.34).Get<int>();

// 调用远程类 TeacherService 中 public void Hello8() 方法
"TeacherService.Hello8".NoWpcParam().Execute(); 

// 调用远程类 TeacherService 中 public string Hello6(TestModel model) 方法, 其中 TestModel 结构如: class {int[] Indexs ,string Name}
"TeacherService.Hello6".WpcParam(new { Indexs = new int[] { 1,2,3,4 }, name="abc" }).Get<string>(); 

// 调用远程类 TeacherService 中 public int Hello4(double value, DateTime time) 方法
"TeacherService.Hello4".WpcParam(new { Value = 12.34, time = DateTime.Now }).Get<int>();

```

### 计划

 - [ ] 完善各种类型的测试.

还在更新中...
