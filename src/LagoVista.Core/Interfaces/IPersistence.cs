// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 6542d8614a2c4fc80f5b1d13c7aa020b52cddf0fdfb12c78eb7031fceef281e0
// IndexVersion: 0
// --- END CODE INDEX META ---
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
