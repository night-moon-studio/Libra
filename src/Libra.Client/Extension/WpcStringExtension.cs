using Libra.Client.Utils;
using System.Threading;

public static class WpcStringExtension
{

    /// <summary>
    /// 远程方法带有参数
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="caller">调用标识,一般由 "类名.方法名"组成</param>
    /// <param name="parameters">方法参数, 多参数请用匿名类包裹</param>
    /// <returns></returns>
    public static LibraExecutor WpcParam<T>(this string caller, T parameters)
    {
        return new LibraExecutor(caller, LibraDefined.DEFAULT_DOMAIN, LibraWirteHandler<T>.Serialize(parameters));

    }


    // <summary>
    /// 远程方法带有参数
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="caller">调用标识,一般由 "类名.方法名"组成</param>
    /// <param name="parameters">方法参数, 多参数请用匿名类包裹</param>
    /// <returns></returns>
    public static LibraExecutor WpcParam<T>(this (string,string) caller, T parameters)
    {
        return new LibraExecutor(caller.Item2, caller.Item1, LibraWirteHandler<T>.Serialize(parameters));

    }


    /// <summary>
    /// 远程方法无参数
    /// </summary>
    /// <param name="caller">调用标识,一般由 "类名.方法名"组成</param>
    /// <returns></returns>
    public static LibraExecutor WpcParam(this string caller)
    {
        return new LibraExecutor(caller, LibraDefined.DEFAULT_DOMAIN, null);
    }


    /// <summary>
    /// 远程方法无参数
    /// </summary>
    /// <param name="caller">调用标识,一般由 "类名.方法名"组成</param>
    /// <returns></returns>
    public static LibraExecutor WpcParam(this (string,string) caller)
    {
        return new LibraExecutor(caller.Item2, caller.Item1, null);
    }

}
