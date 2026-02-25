using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Product
{
    public interface IPublicProductStore
    {
        Task<ProductOffering> GetProductAsync(string orgNs, string productCategyKey, string productKey);
        Task<ProductOffering> GetProductAsync(string orgId, Guid productId);
        Task<ListResponse<ProductOffering>> GetProductsAsync(string orgns, string categoryKey, ListRequest request);
     }
}
