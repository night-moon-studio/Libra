using Libra.Client.Utils;
using System;
using System.Net.Http;

public static class WpcStringExtension
{

    /// <summary>
    ///  远程方法带有参数
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="tuple">调用标识和远程地址,标识一般由 "类名.方法名"组成</param>
    /// <param name="parameters">方法参数, 多参数请用匿名类包裹</param>
    /// <returns></returns>
    public static LibraExecutor WpcParam<T>(this (Uri,string) tuple, T parameters)
    {
        return new LibraExecutorWithUrl<T>(tuple.Item2,tuple.Item1, parameters);
    }


    /// <summary>
    ///  远程方法带有参数
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="tuple">调用标识和远程地址,标识一般由 "类名.方法名"组成</param>
    /// <param name="parameters">方法参数, 多参数请用匿名类包裹</param>
    /// <returns></returns>
    public static LibraExecutor WpcParam<T>(this (string, Uri) tuple, T parameters)
    {
        return new LibraExecutorWithUrl<T>(tuple.Item1, tuple.Item2, parameters);
    }


    /// <summary>
    /// 远程方法带有参数
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="caller">调用标识,一般由 "类名.方法名"组成</param>
    /// <param name="parameters">方法参数, 多参数请用匿名类包裹</param>
    /// <returns></returns>
    public static LibraExecutor WpcParam<T>(this string caller, T parameters)
    {
        return new LibraExecutorWithoutUrl<T>(caller, parameters);
    }


    /// <summary>
    /// 远程方法无参数
    /// </summary>
    /// <param name="caller">调用标识,一般由 "类名.方法名"组成</param>
    /// <returns></returns>
    public static LibraExecutor NoWpcParam(this string caller)
    {
        return new LibraExecutor(caller, null);
    }

    /// <summary>
    /// 远程方法无参数
    /// </summary>
    /// <param name="tuple">调用标识和远程地址,标识一般由 "类名.方法名"组成</param>
    /// <returns></returns>
    public static LibraExecutor NoWpcParam(this (string, Uri) tuple)
    {
        return new LibraExecutorWithUrl(tuple.Item1, tuple.Item2, null);
    }

    /// <summary>
    /// 远程方法无参数
    /// </summary>
    /// <param name="tuple">调用标识和远程地址,标识一般由 "类名.方法名"组成</param>
    /// <returns></returns>
    public static LibraExecutor NoWpcParam(this (Uri, string) tuple)
    {
        return new LibraExecutorWithUrl(tuple.Item2, tuple.Item1, null);
    }

}
