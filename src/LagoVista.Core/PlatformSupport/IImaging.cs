// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: ecd4858b78e9ddcc47870040cf63248fa451971df1c769588bbb43662223f0cf
// IndexVersion: 1
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.PlatformSupport
{
    public interface IImaging
    {
        Task<Stream> ResizeImage(Stream sourceStream, uint maxWidth, uint maxHeight);
    }
}
