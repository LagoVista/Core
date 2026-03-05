using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IKeyVersionResolver
    {
        Task<int> GetKvAsync(string keyId, CancellationToken ct = default);
    }

   
}
