using Libra.Client.Utils;
using System;
using System.Net;
using System.Threading.Tasks;


public static class LibraExecutorExtension
{
    #region 组播通知
    /// <summary>
    /// 通知一组远程主机,并返回通知是否成功
    /// </summary>
    /// <param name="key">组播KEY</param>
    /// <returns></returns>
    public static Task<bool> MulticastNotifyAsync<TBool>(this LibraExecutor executor, string key, params int[] indexs)
    {

        TaskCompletionSource<bool> cts = new TaskCompletionSource<bool>();
        if (indexs.Length == 0)
        {

            Task.Run(() =>
            {

                var hosts = LibraMulticastHostManagement.GetUrls(key);
                Parallel.For(0, hosts.Length, async index =>
                {
                    var host = hosts[index];
                    try
                    {
                        if (!(await executor.GetResultAsync<bool>(host.uri, host.requestHandler).ConfigureAwait(false)))
                        {
                            cts.TrySetResult(false);
                        }
                    }
                    catch
                    {

                        cts.TrySetResult(false);
                    }
                   
                });
                cts.TrySetResult(true);

            });


        }
        else
        {
            Task.Run((Action)(() =>
            {

                var hosts = LibraMulticastHostManagement.GetUrls(key);
                Parallel.For(0, hosts.Length, async index =>
                {
                    var host = hosts[indexs[index]];
                    try
                    {
                        if (!await executor.GetResultAsync<bool>(host.uri, host.requestHandler).ConfigureAwait(false))
                        {
                            cts.TrySetResult(false);
                        }
                    }
                    catch
                    {
                        cts.TrySetResult(false);
                    }
                    
                });
                cts.TrySetResult(true);

            }));

        }
        return cts.Task;

    }


    /// <summary>
    /// 通知一组远程主机,并返回通知是否成功
    /// </summary>
    /// <param name="key">组播KEY</param>
    /// <returns></returns>
    public static Task<bool> MulticastNotifyAsync(this LibraExecutor executor, string key, params int[] indexs)
    {

        TaskCompletionSource<bool> cts = new TaskCompletionSource<bool>();
        if (indexs.Length == 0)
        {

            Task.Run(() =>
            {

                var hosts = LibraMulticastHostManagement.GetUrls(key);
                Parallel.For(0, hosts.Length, async index =>
                {
                    var host = hosts[index];
                    try
                    {
                        var result = await executor.GetCodeAsync(host.uri, host.requestHandler).ConfigureAwait(false);
                        if (result != HttpStatusCode.OK && result != HttpStatusCode.NoContent)
                        {
                            cts.TrySetResult(false);
                        }
                    }
                    catch
                    {
                        cts.TrySetResult(false);
                    }
                    
                });
                cts.TrySetResult(true);

            });


        }
        else
        {
            Task.Run(() =>
            {

                var hosts = LibraMulticastHostManagement.GetUrls(key);
                Parallel.For(0, indexs.Length, async index =>
                {
                    var host = hosts[indexs[index]];
                    try
                    {
                        var result = await executor.GetCodeAsync(host.uri, host.requestHandler).ConfigureAwait(false);
                        if (result != HttpStatusCode.OK && result != HttpStatusCode.NoContent)
                        {
                            cts.TrySetResult(false);
                        }
                    }
                    catch
                    {
                        cts.TrySetResult(false);
                    }
                    
                });
                cts.TrySetResult(true);

            });

        }
        return cts.Task;

    }
    #endregion


    #region 组播返回值
    /// <summary>
    /// 执行一组远程请求,并返回数组结果
    /// </summary>
    /// <param name="key">组播KEY</param>
    /// <returns></returns>
    public static async Task<S[]> MulticastArrayResultAsync<S>(this LibraExecutor executor, string key, params int[] indexs)
    {

        if (indexs.Length == 0)
        {

            var hosts = LibraMulticastHostManagement.GetUrls(key);
            var result = new S[hosts.Length];
            Parallel.For(0, hosts.Length, async index => 
            {
                var host = hosts[index];
                try
                {
                    result[index] = await executor.GetResultAsync<S>(host.uri, host.requestHandler).ConfigureAwait(false);
                }
                catch
                {

                }
                
            });
            return result;

        }
        else
        {

            var hosts = LibraMulticastHostManagement.GetUrls(key);
            var result = new S[hosts.Length];
            Parallel.For(0, indexs.Length, async index => {
                var host = hosts[indexs[index]];
                try
                {
                    result[indexs[index]] = await executor.GetResultAsync<S>(host.uri, host.requestHandler).ConfigureAwait(false);
                }
                catch
                {
                }
                
            });
            return result;

        }

    }


    /// <summary>
    /// 执行一组远程请求,并返回数组结果
    /// </summary>
    /// <param name="key">组播KEY</param>
    /// <returns></returns>
    public static async Task<HttpStatusCode[]> MulticastArrayResultAsync(this LibraExecutor executor, string key, params int[] indexs)
    {

        if (indexs.Length == 0)
        {

            var hosts = LibraMulticastHostManagement.GetUrls(key);
            var result = new HttpStatusCode[hosts.Length];
            Parallel.For(0, hosts.Length, async index =>
            {
                var host = hosts[indexs[index]];
                try
                {
                    result[index] = await executor.GetCodeAsync((Uri)host.uri, (Action<System.Net.Http.HttpRequestMessage>)host.requestHandler).ConfigureAwait(false);
                }
                catch
                {

                }
               
            });
            return result;

        }
        else
        {

            var hosts = LibraMulticastHostManagement.GetUrls(key);
            var result = new HttpStatusCode[hosts.Length];
            Parallel.For(0, indexs.Length, (Action<int>)(async index => {
                var host = hosts[indexs[index]];
                try
                {
                    result[indexs[index]] = await executor.GetCodeAsync((Uri)host.uri, (Action<System.Net.Http.HttpRequestMessage>)host.requestHandler).ConfigureAwait(false);
                }
                catch
                {

                  
                }
                
            }));
            return result;

        }

    }


    /// <summary>
    /// 执行一组远程请求,并返回元祖数组结果
    /// </summary>
    /// <param name="key">组播KEY</param>
    /// <returns></returns>
    public static async Task<LibraMulticastResult<S>[]> MulticastTupleResultAsync<S>(this LibraExecutor executor, string key, params int[] indexs)
    {

        if (indexs.Length == 0)
        {

            var hosts = LibraMulticastHostManagement.GetUrls(key);
            var result = new LibraMulticastResult<S>[hosts.Length];
            Parallel.For(0, hosts.Length, async index =>
            {
                var host = hosts[index];
                try
                {
                    result[index] = new LibraMulticastResult<S>(host.uri.Authority, await executor.GetResultAsync<S>(host.uri, host.requestHandler).ConfigureAwait(false));
                }
                catch 
                {

                }
                

            });
            return result;

        }
        else
        {

            var hosts = LibraMulticastHostManagement.GetUrls(key);
            var result = new LibraMulticastResult<S>[indexs.Length];
            Parallel.For(0, indexs.Length, async index =>
            {
                var host = hosts[indexs[index]];
                try
                {
                    result[indexs[index]] = new LibraMulticastResult<S>(host.uri.Authority, await executor.GetResultAsync<S>(host.uri, host.requestHandler).ConfigureAwait(false));
                }
                catch 
                {
                }
                
            });
            return result;
        }

    }



    /// <summary>
    /// 执行一组远程请求,并返回元祖数组结果
    /// </summary>
    /// <param name="key">组播KEY</param>
    /// <returns></returns>
    public static async Task<LibraMulticastResult[]> MulticastTupleResult(this LibraExecutor executor, string key, params int[] indexs)
    {

        if (indexs.Length == 0)
        {

            var hosts = LibraMulticastHostManagement.GetUrls(key);
            var result = new LibraMulticastResult[hosts.Length];
            Parallel.For(0, hosts.Length, async index =>
            {
                var host = hosts[index];
                try
                {
                    result[index] = new LibraMulticastResult(host.uri.Authority, await executor.GetCodeAsync(host.uri, host.requestHandler).ConfigureAwait(false));
                }
                catch 
                {

                }
                

            });
            return result;

        }
        else
        {

            var hosts = LibraMulticastHostManagement.GetUrls(key);
            var result = new LibraMulticastResult[indexs.Length];
            Parallel.For(0, indexs.Length, async index =>
            {
                var host = hosts[indexs[index]];
                try
                {
                    result[indexs[index]] = new LibraMulticastResult(host.uri.Authority, await executor.GetCodeAsync(host.uri, host.requestHandler).ConfigureAwait(false));
                }
                catch
                {

                }
                
            });
            return result;
        }

    }
    #endregion

}
