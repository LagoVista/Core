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
