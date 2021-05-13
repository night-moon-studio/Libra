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
        private readonly ConcurrentDictionary<string, MulticastModel> _urlList;
        public readonly string MulticastKey;

        /// <summary>
        /// 创建一个多播主机群
        /// </summary>
        /// <param name="name">多播KEY</param>
        public LibraMulticastHost(string key)
        {
            
            MulticastKey = key;
            _urlList = new ConcurrentDictionary<string, MulticastModel>();
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
                    _urlList[url] = (new Uri(url), null);

                }
                SyncUris();
            }
        }
        /// <summary>
        /// 按顺序追加若干主机
        /// </summary>
        /// <param name="urls"></param>
        public void AppendHosts(params (string uri, Action<HttpRequestMessage> requestHandler)[] hosts)
        {
            if (hosts == null)
            {
                return;
            }
            lock (_multicastLock)
            {

                for (int i = 0; i < hosts.Length; i++)
                {

                    var url = hosts[i].uri;
                    if (_urlList.TryGetValue(url, out var model))
                    {
                        model.RequestHandler = hosts[i].requestHandler;

                    }
                    else
                    {
                        _urlList[url] = hosts[i];
                    }

                }
                SyncUris();
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

                if (_urlList.TryGetValue(url, out var model))
                {
                    model.RequestHandler = requestHandler;
                    
                }
                else
                {
                    _urlList[url] = new MulticastModel(new Uri(url),requestHandler);
                }
                SyncUris();
            }

        }


        /// <summary>
        /// 向 Management 同步资源
        /// </summary>
        private void SyncUris()
        {
            LibraMulticastHostManagement.SetMapper(MulticastKey, _urlList.Values.ToArray());
        }

    }


    public class MulticastModel 
    {
        public MulticastModel(Uri uri, Action<HttpRequestMessage> action = null)
        {
            Address = uri;
            RequestHandler = action;
        }
        public readonly Uri Address;
        public Action<HttpRequestMessage> RequestHandler;

        public static implicit operator MulticastModel(in (Uri, Action<HttpRequestMessage>) model)
        {
            return new MulticastModel(model.Item1, model.Item2);
        }
        public static implicit operator MulticastModel(in (Action<HttpRequestMessage> , Uri) model)
        {
            return new MulticastModel(model.Item2, model.Item1);
        }

        public static implicit operator MulticastModel((string uri, Action<HttpRequestMessage> requestHandler) model)
        {
            return new MulticastModel(new Uri(model.uri), model.requestHandler);
        }
    }


}

