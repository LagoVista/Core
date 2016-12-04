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
