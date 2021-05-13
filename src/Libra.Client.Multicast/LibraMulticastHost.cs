using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Libra.Client.Multicast
{

    /// <summary>
    /// 多播主机操作类,该类的操作将被记录到 Management
    /// </summary>
    public class LibraMulticastHost
    {

        private readonly object _multicastLock = new object();
        private readonly ConcurrentDictionary<string, LibraMulticastModel> _callerInfoCache;
        public readonly string MulticastKey;

        /// <summary>
        /// 创建一个多播主机群
        /// </summary>
        /// <param name="name">多播KEY</param>
        public LibraMulticastHost(string key)
        {
            
            MulticastKey = key;
            _callerInfoCache = new ConcurrentDictionary<string, LibraMulticastModel>();
        }


        /// <summary>
        /// 按顺序追加若干主机
        /// </summary>
        /// <param name="urls"></param>
        public void AppendHosts(params string[] urls)
        {
            if (urls == null)
            {
                return;
            }
            lock (_multicastLock)
            {

                for (int i = 0; i < urls.Length; i++)
                {

                    var url = urls[i];
                    _callerInfoCache[url] = (new Uri(url), null);

                }
            }
        }


        /// <summary>
        /// 按顺序追加若干主机
        /// </summary>
        /// <param name="urls"></param>
        public void AppendHosts(params (string, Action<HttpRequestMessage>)[] callerInfos)
        {
            if (callerInfos == null)
            {
                return;
            }
            lock (_multicastLock)
            {

                for (int i = 0; i < callerInfos.Length; i++)
                {

                    var url = callerInfos[i].Item1;
                    if (_callerInfoCache.TryGetValue(url, out var model))
                    {
                        model.RequestHandler = callerInfos[i].Item2;

                    }
                    else
                    {
                        _callerInfoCache[url] = callerInfos[i];
                    }

                }
            }
        }


        /// <summary>
        /// 按顺序追加若干主机
        /// </summary>
        /// <param name="urls"></param>
        public void AppendHosts(IEnumerable<(string, Action<HttpRequestMessage>)> callerInfos)
        {
            if (callerInfos == null)
            {
                return;
            }
            lock (_multicastLock)
            {

                foreach (var item in callerInfos)
                {
                    var url = item.Item1;
                    if (_callerInfoCache.TryGetValue(url, out var model))
                    {
                        model.RequestHandler = item.Item2;

                    }
                    else
                    {
                        _callerInfoCache[url] = item;
                    }
                }

            }
        }


        /// <summary>
        /// 按顺序追加一个主机
        /// </summary>
        /// <param name="url"></param>
        public void AppendHost(string url, Action<HttpRequestMessage> requestHandler = null)
        {

            lock (_multicastLock)
            {

                if (_callerInfoCache.TryGetValue(url, out var model))
                {
                    model.RequestHandler = requestHandler;
                    
                }
                else
                {
                    _callerInfoCache[url] = new LibraMulticastModel(new Uri(url),requestHandler);
                }
            }

        }


        /// <summary>
        /// 向 Management 同步资源
        /// </summary>
        public void Save()
        {
            LibraMulticastHostManagement.SetMapper(MulticastKey, _callerInfoCache.Values.ToArray());
        }

    }

}
public class LibraMulticastModel
{
    public LibraMulticastModel(Uri uri, Action<HttpRequestMessage> action = null)
    {
        Address = uri;
        RequestHandler = action;
    }
    public readonly Uri Address;
    public Action<HttpRequestMessage> RequestHandler;

    public static implicit operator LibraMulticastModel(in (Uri, Action<HttpRequestMessage>) model)
    {
        return new LibraMulticastModel(model.Item1, model.Item2);
    }
    public static implicit operator LibraMulticastModel(in (Action<HttpRequestMessage>, Uri) model)
    {
        return new LibraMulticastModel(model.Item2, model.Item1);
    }

    public static implicit operator LibraMulticastModel((string uri, Action<HttpRequestMessage> requestHandler) model)
    {
        return new LibraMulticastModel(new Uri(model.uri), model.requestHandler);
    }
}
