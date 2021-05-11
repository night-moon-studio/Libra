using Libra.Client.Multicast;
using Libra.Client.Utils;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;


public static class LibraExecutorExtension
{
    #region 组播通知
    /// <summary>
    /// 通知一组远程主机,并返回通知是否成功
    /// </summary>
    /// <param name="key">组播KEY</param>
    /// <returns></returns>
    public static LibraMulticastTask<bool> MulticastNotifyAsync<TBool>(this LibraExecutor executor, string key, params int[] indexs)
    {

        var hosts = LibraMulticastHostManagement.GetUrls(key);
        if (indexs.Length == 0)
        {

            var task = LibraMulticastTask<bool>.Create(hosts.Length);
            Parallel.For(0, hosts.Length, async index =>
            {
                var host = hosts[index];
                try
                {
                    if (!(await executor.GetResultAsync<bool>(host.uri, host.requestHandler).ConfigureAwait(false)))
                    {
                        task.SetOnceResult(false);
                    }
                    else
                    {
                        task.FillResult(true);
                    }
                }
                catch
                {

                    task.SetOnceResult(false);
                }

            });
            return task;
        }
        else
        {

            var task = LibraMulticastTask<bool>.Create(indexs.Length);
            Parallel.For(0, hosts.Length, async index =>
            {
                var host = hosts[indexs[index]];
                try
                {
                    if (!await executor.GetResultAsync<bool>(host.uri, host.requestHandler).ConfigureAwait(false))
                    {
                        task.SetOnceResult(false);
                    }
                    else
                    {
                        task.FillResult(true);
                    }
                }
                catch
                {
                    task.SetOnceResult(false);
                }
            });
            return task;
        }
       

    }


    /// <summary>
    /// 通知一组远程主机,并返回通知是否成功
    /// </summary>
    /// <param name="key">组播KEY</param>
    /// <returns></returns>
    public static LibraMulticastTask<bool> MulticastNotifyAsync(this LibraExecutor executor, string key, params int[] indexs)
    {

        var hosts = LibraMulticastHostManagement.GetUrls(key);
       
        if (indexs.Length == 0)
        {
            var task = LibraMulticastTask<bool>.Create(hosts.Length);
            Parallel.For(0, hosts.Length, async index =>
            {
                var host = hosts[index];
                try
                {
                    var result = await executor.GetCodeAsync(host.uri, host.requestHandler).ConfigureAwait(false);
                    if (result != HttpStatusCode.OK && result != HttpStatusCode.NoContent)
                    {
                        task.SetOnceResult(false);
                    }
                    else
                    {
                        task.FillResult(true);
                    }
                }
                catch
                {
                    task.SetOnceResult(false);
                }

            });
            return task;
        }
        else
        {
            var task = LibraMulticastTask<bool>.Create(indexs.Length);
            Parallel.For(0, indexs.Length, async index =>
            {
                var host = hosts[indexs[index]];
                try
                {
                    var result = await executor.GetCodeAsync(host.uri, host.requestHandler).ConfigureAwait(false);
                    if (result != HttpStatusCode.OK && result != HttpStatusCode.NoContent)
                    {
                        task.SetOnceResult(false);
                    }
                    else
                    {
                        task.FillResult(true);
                    }
                }
                catch
                {
                    task.SetOnceResult(false);
                }

            });
            return task;
        }

    }
    #endregion


    #region 异步组播返回值
    /// <summary>
    /// 执行一组远程请求,并返回数组结果
    /// </summary>
    /// <param name="key">组播KEY</param>
    /// <returns></returns>
    public static LibraMulticastTask<S[]> MulticastArrayResultAsync<S>(this LibraExecutor executor, string key, params int[] indexs)
    {

        if (indexs.Length == 0)
        {
            var hosts = LibraMulticastHostManagement.GetUrls(key);
            var task = LibraMulticastTask<S[]>.Create(hosts.Length);
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
                task.FillResult(result);
            });
            return task;
        }
        else
        {
            var hosts = LibraMulticastHostManagement.GetUrls(key);
            var task = LibraMulticastTask<S[]>.Create(indexs.Length);
            var result = new S[indexs.Length];
            Parallel.For(0, indexs.Length, async index =>
            {
                var host = hosts[indexs[index]];
                try
                {
                    result[indexs[index]] = await executor.GetResultAsync<S>(host.uri, host.requestHandler).ConfigureAwait(false);
                }
                catch
                {
                }
                task.FillResult(result);
            });
            return task;
        }

    }


    /// <summary>
    /// 执行一组远程请求,并返回数组结果
    /// </summary>
    /// <param name="key">组播KEY</param>
    /// <returns></returns>
    public static LibraMulticastTask<HttpStatusCode[]> MulticastArrayResultAsync(this LibraExecutor executor, string key, params int[] indexs)
    {

        if (indexs.Length == 0)
        {

            var hosts = LibraMulticastHostManagement.GetUrls(key);
            var task = LibraMulticastTask<HttpStatusCode[]>.Create(hosts.Length);
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
                task.FillResult(result);
            });
            return task;

        }
        else
        {

            var hosts = LibraMulticastHostManagement.GetUrls(key);
            var task = LibraMulticastTask<HttpStatusCode[]>.Create(indexs.Length);
            var result = new HttpStatusCode[indexs.Length];
            Parallel.For(0, indexs.Length, async index =>
            {
                var host = hosts[indexs[index]];
                try
                {
                    result[indexs[index]] = await executor.GetCodeAsync(host.uri, host.requestHandler).ConfigureAwait(false);
                }
                catch
                {


                }
                task.FillResult(result);
            });
            return task;

        }

    }


    /// <summary>
    /// 执行一组远程请求,并返回元祖数组结果
    /// </summary>
    /// <param name="key">组播KEY</param>
    /// <returns></returns>
    public static LibraMulticastTask<LibraMulticastResult<S>[]> MulticastTupleResultAsync<S>(this LibraExecutor executor, string key, params int[] indexs)
    {

        if (indexs.Length == 0)
        {

            var hosts = LibraMulticastHostManagement.GetUrls(key);
            var task = LibraMulticastTask<LibraMulticastResult<S>[]>.Create(hosts.Length);
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
                task.FillResult(result);

            });
            return task;

        }
        else
        {

            var hosts = LibraMulticastHostManagement.GetUrls(key);
            var task = LibraMulticastTask<LibraMulticastResult<S>[]>.Create(indexs.Length);
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
                task.FillResult(result);
            });
            return task;
        }

    }
    #endregion

}
