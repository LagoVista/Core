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
