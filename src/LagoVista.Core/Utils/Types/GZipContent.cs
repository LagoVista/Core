using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Utils.Types
{

    /// <summary>
    /// Minimal GZip wrapper for HttpContent. Sets Content-Encoding: gzip.
    /// </summary>
    public sealed class GZipContent : HttpContent
    {
        private readonly HttpContent _inner;
        public GZipContent(HttpContent inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            Headers.ContentType = inner.Headers.ContentType;
            Headers.ContentEncoding.Add("gzip");
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            using (var gzip = new GZipStream(stream, CompressionLevel.Fastest, leaveOpen: true))
            {
                await _inner.CopyToAsync(gzip).ConfigureAwait(false);
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }
}
