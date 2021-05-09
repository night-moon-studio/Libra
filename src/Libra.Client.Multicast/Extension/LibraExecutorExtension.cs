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
                Parallel.For(0, hosts.Length, index =>
                {
                    var host = hosts[index];
                    if (!executor.GetResult<bool>(host.uri, host.requestHandler))
                    {
                        cts.SetResult(false);
                    }
                });
                cts.SetResult(true);

            });


        }
        else
        {
            Task.Run((Action)(() =>
            {

                var hosts = LibraMulticastHostManagement.GetUrls(key);
                Parallel.For(0, hosts.Length, index =>
                {
                    var host = hosts[indexs[index]];
                    if (!executor.GetResult<bool>(host.uri, host.requestHandler))
                    {
                        cts.SetResult(false);
                    }
                });
                cts.SetResult(true);

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
                Parallel.For(0, hosts.Length, index =>
                {
                    var host = hosts[index];
                    var result = executor.GetCode(host.uri,host.requestHandler);
                    if (result != HttpStatusCode.OK && result != HttpStatusCode.NoContent)
                    {
                        cts.SetResult(false);
                    }
                });
                cts.SetResult(true);

            });


        }
        else
        {
            Task.Run(() =>
            {

                var hosts = LibraMulticastHostManagement.GetUrls(key);
                Parallel.For(0, indexs.Length, index =>
                {
                    var host = hosts[indexs[index]];
                    var result = executor.GetCode(host.uri, host.requestHandler);
                    if (result != HttpStatusCode.OK && result != HttpStatusCode.NoContent)
                    {
                        cts.SetResult(false);
                    }
                });
                cts.SetResult(true);

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
    public static S[] MulticastArrayResult<S>(this LibraExecutor executor, string key, params int[] indexs)
    {

        if (indexs.Length == 0)
        {

            var hosts = LibraMulticastHostManagement.GetUrls(key);
            var result = new S[hosts.Length];
            Parallel.For(0, hosts.Length, index => 
            {
                var host = hosts[index];
                result[index] = executor.GetResult<S>(host.uri, host.requestHandler); 
            });
            return result;

        }
        else
        {

            var hosts = LibraMulticastHostManagement.GetUrls(key);
            var result = new S[hosts.Length];
            Parallel.For(0, indexs.Length, index => {
                var host = hosts[indexs[index]];
                result[indexs[index]] = executor.GetResult<S>(host.uri, host.requestHandler); 
            });
            return result;

        }

    }


    /// <summary>
    /// 执行一组远程请求,并返回数组结果
    /// </summary>
    /// <param name="key">组播KEY</param>
    /// <returns></returns>
    public static HttpStatusCode[] MulticastArrayResult(this LibraExecutor executor, string key, params int[] indexs)
    {

        if (indexs.Length == 0)
        {

            var hosts = LibraMulticastHostManagement.GetUrls(key);
            var result = new HttpStatusCode[hosts.Length];
            Parallel.For(0, hosts.Length, index => {
                var host = hosts[indexs[index]];
                result[index] = executor.GetCode(host.uri, host.requestHandler); 
            });
            return result;

        }
        else
        {

            var hosts = LibraMulticastHostManagement.GetUrls(key);
            var result = new HttpStatusCode[hosts.Length];
            Parallel.For(0, indexs.Length, index => {
                var host = hosts[indexs[index]];
                result[indexs[index]] = executor.GetCode(host.uri, host.requestHandler); 
            });
            return result;

        }

    }


    /// <summary>
    /// 执行一组远程请求,并返回元祖数组结果
    /// </summary>
    /// <param name="key">组播KEY</param>
    /// <returns></returns>
    public static LibraMulticastResult<S>[] MulticastTupleResult<S>(this LibraExecutor executor, string key, params int[] indexs)
    {

        if (indexs.Length == 0)
        {

            var hosts = LibraMulticastHostManagement.GetUrls(key);
            var result = new LibraMulticastResult<S>[hosts.Length];
            Parallel.For(0, hosts.Length, index =>
            {
                var host = hosts[index];
                result[index] = new LibraMulticastResult<S>(host.uri.Authority, executor.GetResult<S>(host.uri, host.requestHandler));

            });
            return result;

        }
        else
        {

            var hosts = LibraMulticastHostManagement.GetUrls(key);
            var result = new LibraMulticastResult<S>[indexs.Length];
            Parallel.For(0, indexs.Length, index =>
            {
                var host = hosts[indexs[index]];
                result[indexs[index]] = new LibraMulticastResult<S>(host.uri.Authority, executor.GetResult<S>(host.uri, host.requestHandler));
            });
            return result;
        }

    }



    /// <summary>
    /// 执行一组远程请求,并返回元祖数组结果
    /// </summary>
    /// <param name="key">组播KEY</param>
    /// <returns></returns>
    public static LibraMulticastResult[] MulticastTupleResult(this LibraExecutor executor, string key, params int[] indexs)
    {

        if (indexs.Length == 0)
        {

            var hosts = LibraMulticastHostManagement.GetUrls(key);
            var result = new LibraMulticastResult[hosts.Length];
            Parallel.For(0, hosts.Length, index =>
            {
                var host = hosts[index];
                result[index] = new LibraMulticastResult(host.uri.Authority, executor.GetCode(host.uri, host.requestHandler));

            });
            return result;

        }
        else
        {

            var hosts = LibraMulticastHostManagement.GetUrls(key);
            var result = new LibraMulticastResult[indexs.Length];
            Parallel.For(0, indexs.Length, index =>
            {
                var host = hosts[indexs[index]];
                result[indexs[index]] = new LibraMulticastResult(host.uri.Authority, executor.GetCode(host.uri, host.requestHandler));
            });
            return result;
        }

    }
    #endregion

}
