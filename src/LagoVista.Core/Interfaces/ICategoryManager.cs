// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: caf6f6229812545060b76cb51bb01982b001b3c3f3c3cf1602a89e1050ff0f31
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface ICategoryManager
    {
        Task<InvokeResult> AddCategoryAsync(Category category, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteCategoryAsync(string id, EntityHeader org, EntityHeader user);
        Task<Category> GetCategoryAsync(string id, EntityHeader org, EntityHeader user);
        Task<ListResponse<Category>> GetCategoriesAsync(string categoryType, ListRequest listRequest, EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateCategoryAsync(Category category, EntityHeader org, EntityHeader user);
    }
}
