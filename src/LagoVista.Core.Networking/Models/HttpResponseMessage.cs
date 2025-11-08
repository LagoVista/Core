// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 3073f3183a555086d90fe23564aeae80444f333536b9f8e69877d56e0702fed9
// IndexVersion: 2
// --- END CODE INDEX META ---
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Models
{
    public class HttpResponseMessage
    {
        public int ResponseCode { get; set; }
        public String ContentType { get; set; }
        public String Content { get; set; }


        public HttpResponseMessage(Stream outputStream)
        {
            OuptutStream = outputStream;
            ResponseCode = 200;
            ContentType = "application/json";
            Content = String.Empty;
        }

        public Stream OuptutStream
        {
            get; private set;
        }

        public async Task Send()
        {
            var bodyArray = Encoding.UTF8.GetBytes(Content);
            using (var stream = new MemoryStream(bodyArray))
            {
                var header = String.Format("HTTP/1.1 {0} OK\r\n" +
                                  "Content-Length: {1}\r\n" +
                                  "Content-Type: {2}\r\n" + 
                                  "Connection: close\r\n\r\n",
                                  ResponseCode, 
                                  stream.Length,
                                  ContentType);
                var headerArray = Encoding.UTF8.GetBytes(header);
                await OuptutStream.WriteAsync(headerArray, 0, headerArray.Length);
                await stream.CopyToAsync(OuptutStream);
                await OuptutStream.FlushAsync();
            }
        }
    }

    public class HttpResponseMessage<T> : HttpResponseMessage
    {
        public HttpResponseMessage(Stream outputStream) : base(outputStream)
        {

        }

        private T _payload;
        public T Payload
        {
            get { return _payload; }
            set
            {
                _payload = value;
                Content = JsonConvert.SerializeObject(_payload);
            }
        }
    }
}
