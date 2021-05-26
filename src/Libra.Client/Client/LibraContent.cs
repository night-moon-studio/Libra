using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Libra.Sender
{

    /// <summary>
    /// body 内容协议类(感谢 WebApiClient 作者)
    /// </summary>
    internal class LibraContent : HttpContent
    {
       
        private readonly static MediaTypeHeaderValue _contentType;
        //private LibraProtocal _protocal;
        static LibraContent()
        {
            _contentType = new MediaTypeHeaderValue("application/json");
            _contentType.CharSet = "UTF-8";
        }

        public LibraContent()
        {
            Headers.ContentType = _contentType;
        }


        //public LibraProtocal Protocal 
        //{ 
        //    set
        //    {
        //        _protocal = value;
        //    }
        //}

        public Func<Stream, Task> ProtocalAction;


        /// <summary>
        /// 序列化直接写到流里,感谢 WebApiClient 作者
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context) =>
            await ProtocalAction(stream).ConfigureAwait(false);


        /// <summary>
        /// 不检查长度,自动填充,感谢 WebApiClient 作者
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }
    }
}
