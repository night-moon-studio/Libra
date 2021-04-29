using System;
using System.Collections.Generic;
using System.Linq;

namespace Libra.Multicast
{

    /// <summary>
    /// 多播主机操作类,该类的操作将被记录到 Management
    /// </summary>
    public class LibraMulticastHost
    {

        private readonly object _multicastLock = new object();
        private readonly HashSet<string> _urlList;
        public readonly List<Uri> Urls;
        public readonly string MulticastKey;

        /// <summary>
        /// 创建一个多播主机群
        /// </summary>
        /// <param name="name">多播KEY</param>
        public LibraMulticastHost(string key)
        {
            
            MulticastKey = key;
            _urlList = new HashSet<string>();
            Urls = new List<Uri>();
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
                    var url = urls[i] + (urls[i].EndsWith('/') ? "Libra" : "/Libra");
                    if (_urlList.Add(url))
                    {
                        Urls.Add(new Uri(url));
                    }
                }
                SyncUris();
            }
        }


        /// <summary>
        /// 按顺序追加一个主机
        /// </summary>
        /// <param name="url"></param>
        public void AppendHost(string url)
        {

            lock (_multicastLock)
            {

                url += (url.EndsWith('/') ? "Libra" : "/Libra");
                if (_urlList.Add(url))
                {
                    Urls.Add(new Uri(url));
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

