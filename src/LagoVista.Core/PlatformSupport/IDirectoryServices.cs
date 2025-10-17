// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 2a655fd95994670c2fcacaf253d75bffd7a37811fd03fbe3271374320be6907c
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.PlatformSupport
{
    public interface IDirectoryServices
    {
        List<Folder> GetFolders(String parent);
        List<File> GetFiles(String directory);
    }
}
