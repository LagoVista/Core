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
