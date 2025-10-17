// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 04ea28b5560cb0776ce0e4fe40af162c0e518dbaf76390d473572ee799c42751
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IStaticPageStorage
    {   
        Task<InvokeResult<string>> StorePageAsync(string orgId, string folder, string htmlContent);
        Task<InvokeResult<string>> GetPageAsync(string orgId, string folder, string pageId);
    }
}
