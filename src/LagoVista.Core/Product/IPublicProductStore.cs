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
        Task<ProductOffering> GetProductOfferingAsync(string orgId, GuidString36 productId);
        Task<ProductOffering> GetSystemProductAsync(string catgkey, string productKey);
        Task<ListResponse<ProductOffering>> GetProductsAsync(string orgns, string categoryKey, ListRequest request);
     }
}
