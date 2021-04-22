using Libra.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Libra
{

    public class LibraContent : HttpContent
    {
       
        private byte[] _content;
        private int _count;
        private readonly static MediaTypeHeaderValue _contentType;
        static LibraContent()
        {
            _contentType = new MediaTypeHeaderValue("application/json");
            _contentType.CharSet = "UTF-8";
        }
        public LibraContent()
        {
            Headers.ContentType = _contentType;
        }


        public LibraProtocal Protocal 
        { 
            set
            {
                _content = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(value);
                _count = _content.Length;
                Headers.ContentLength = _count;
            }
        }



        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) =>
            stream.WriteAsync(_content, 0, _count);


        protected override bool TryComputeLength(out long length)
        {
            length = _count;
            return true;
        }
    }
}
