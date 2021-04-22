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
        public string[] Urls;
        public string MulticastKey;

        /// <summary>
        /// 创建一个多播主机群
        /// </summary>
        /// <param name="name">多播KEY</param>
        public LibraMulticastHost(string key)
        {
            MulticastKey = key;
            _urlList = new HashSet<string>();
        }


        /// <summary>
        /// 添加若干主机
        /// </summary>
        /// <param name="urls"></param>
        public void AddHosts(params string[] urls)
        {
            if (urls == null)
            {
                return;
            }
            lock (_multicastLock)
            {
                _urlList.UnionWith(urls.Select(item => item + (item.EndsWith('/') ? "Libra" : "/Libra")));
                SyncUris();
            }
        }


        /// <summary>
        /// 添加一个主机
        /// </summary>
        /// <param name="url"></param>
        public void AddHost(string url)
        {
            lock (_multicastLock)
            {
                if (_urlList.Add(url + (url.EndsWith('/') ? "Libra" : "/Libra")))
                {
                    SyncUris();
                }
            }
        }


        /// <summary>
        /// 向 Management 同步资源
        /// </summary>
        private void SyncUris()
        {
            LibraMulticastHostManagement.SetMapper(MulticastKey, _urlList.Select(item => (new Uri(item))).ToArray());
        }

    }

}

