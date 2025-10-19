// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 8049c272fbfd62e88df1d25c6795b8927aff709ecd6382a0c0fbe7ef2f860e00
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.PlatformSupport
{
    public interface ITableStorage
    {
        Task InsertOrReplaceAsync(TableStorageEntity entity);
    }
}
