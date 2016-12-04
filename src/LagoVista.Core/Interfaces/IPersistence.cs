using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IPersistence
    {
        Task WriteStream(String fileName, Stream data);

        Task<Boolean> FileExists(String fileName);

        Task<Boolean> DirectoryExists(String directory);

        Task CreateDirectory(String directory);

        Task DeleteFile(String file);

        Stream ReadStream(String fileName);
    }
}
