using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Libra.Client.Multicast
{

    /// <summary>
    /// 多播主机操作类,该类的操作将被记录到 Management
    /// </summary>
    public class LibraMulticastHost
    {

        private readonly object _multicastLock = new object();
        private readonly HashSet<string> _urlList;
        public readonly List<(Uri uri, Action<HttpRequestMessage> requestHandler)> Urls;
        public readonly string MulticastKey;

        /// <summary>
        /// 创建一个多播主机群
        /// </summary>
        /// <param name="name">多播KEY</param>
        public LibraMulticastHost(string key)
        {
            
            MulticastKey = key;
            _urlList = new HashSet<string>();
            Urls = new List<(Uri uri, Action<HttpRequestMessage> requestHandler)>();
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
                    if (_urlList.Add(url))
                    {
                        Urls.Add((new Uri(url),null));
                    }

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

                    var host = hosts[i];
                    if (_urlList.Add(host.uri))
                    {
                        Urls.Add((new Uri(host.uri),host.requestHandler));
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

                if (_urlList.Add(url))
                {
                    Urls.Add((new Uri(url), requestHandler));
                    SyncUris();
                }

            }

        }


        /// <summary>
        /// 向 Management 同步资源
        /// </summary>
        private void SyncUris()
        {
            LibraMulticastHostManagement.SetMapper(MulticastKey, Urls.ToArray());
        }

    }

}

