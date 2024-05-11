using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IStaticPageStorage
    {
        Task<InvokeResult<string>> StorePageAsync(string htmlContent);
        Task<InvokeResult<string>> GetPageAsync(string pageId);
    }
}
