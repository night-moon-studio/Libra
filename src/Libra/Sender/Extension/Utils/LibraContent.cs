using Libra.Model;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Libra
{

    public class LibraContent : HttpContent
    {
       
        private readonly static MediaTypeHeaderValue _contentType;
        private LibraProtocal _protocal;
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
                _protocal = value;
            }
        }



        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) =>
            JsonSerializer.SerializeAsync(stream,_protocal);



        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }
    }
}
