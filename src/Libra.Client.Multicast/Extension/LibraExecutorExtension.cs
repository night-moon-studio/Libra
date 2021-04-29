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

                var urls = LibraMulticastHostManagement.GetUrls(key);
                Parallel.For(0, urls.Length, index =>
                {
                    if (!executor.GetResult<bool>(urls[index]))
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

                var urls = LibraMulticastHostManagement.GetUrls(key);
                Parallel.For(0, urls.Length, index =>
                {
                    if (!executor.GetResult<bool>(urls[indexs[index]]))
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

                var urls = LibraMulticastHostManagement.GetUrls(key);
                Parallel.For(0, urls.Length, index =>
                {
                    var result = executor.GetCode(urls[index]);
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

                var urls = LibraMulticastHostManagement.GetUrls(key);
                Parallel.For(0, indexs.Length, index =>
                {
                    var result = executor.GetCode(urls[indexs[index]]);
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

            var urls = LibraMulticastHostManagement.GetUrls(key);
            var result = new S[urls.Length];
            Parallel.For(0, urls.Length, index => { result[index] = executor.GetResult<S>(urls[index]); });
            return result;

        }
        else
        {

            var urls = LibraMulticastHostManagement.GetUrls(key);
            var result = new S[urls.Length];
            Parallel.For(0, indexs.Length, index => { result[indexs[index]] = executor.GetResult<S>(urls[indexs[index]]); });
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

            var urls = LibraMulticastHostManagement.GetUrls(key);
            var result = new HttpStatusCode[urls.Length];
            Parallel.For(0, urls.Length, index => { result[index] = executor.GetCode(urls[index]); });
            return result;

        }
        else
        {

            var urls = LibraMulticastHostManagement.GetUrls(key);
            var result = new HttpStatusCode[urls.Length];
            Parallel.For(0, indexs.Length, index => { result[indexs[index]] = executor.GetCode(urls[indexs[index]]); });
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

            var urls = LibraMulticastHostManagement.GetUrls(key);
            var result = new LibraMulticastResult<S>[urls.Length];
            Parallel.For(0, urls.Length, index =>
            {
                var url = urls[index];
                result[index] = new LibraMulticastResult<S>(url.Authority, executor.GetResult<S>(url));

            });
            return result;

        }
        else
        {

            var urls = LibraMulticastHostManagement.GetUrls(key);
            var result = new LibraMulticastResult<S>[indexs.Length];
            Parallel.For(0, indexs.Length, index =>
            {
                var url = urls[indexs[index]];
                result[indexs[index]] = new LibraMulticastResult<S>(url.Authority, executor.GetResult<S>(url));
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

            var urls = LibraMulticastHostManagement.GetUrls(key);
            var result = new LibraMulticastResult[urls.Length];
            Parallel.For(0, urls.Length, index =>
            {
                var url = urls[index];
                result[index] = new LibraMulticastResult(url.Authority, executor.GetCode(url));

            });
            return result;

        }
        else
        {

            var urls = LibraMulticastHostManagement.GetUrls(key);
            var result = new LibraMulticastResult[indexs.Length];
            Parallel.For(0, indexs.Length, index =>
            {
                var url = urls[indexs[index]];
                result[indexs[index]] = new LibraMulticastResult(url.Authority, executor.GetCode(url));
            });
            return result;
        }

    }
    #endregion

}
