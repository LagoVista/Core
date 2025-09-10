using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IStorageUtils
    {
        Task<TEntity> FindWithKeyAsync<TEntity>(string key, IEntityHeader org, bool throwOnNotFound = true) where TEntity : class, IIDEntity, INoSQLEntity, IKeyedEntity, IOwnedEntity;
        Task<TEntity> FindWithKeyAsync<TEntity>(string key) where TEntity : class, IIDEntity, INoSQLEntity, IKeyedEntity, IOwnedEntity;
        Task<List<TEntity>> FindByTypeAsync<TEntity>(string objectType, IEntityHeader org) where TEntity : class, IIDEntity, INamedEntity, IOwnedEntity, INoSQLEntity, IKeyedEntity;
        Task<IStandardModel> FindWithKeyAsync(string objectType, string key, IEntityHeader org, bool throwOnNotFound = true);
        Task<TEntity> FindWithIdAsync<TEntity>(string id, string ownerId) where TEntity : class, IIDEntity, INoSQLEntity, IKeyedEntity, IOwnedEntity;
        Task DeleteByKeyIfExistsAsync<TEntity>(string key, IEntityHeader org) where TEntity : class, IIDEntity, INoSQLEntity, IKeyedEntity, IOwnedEntity;
        Task DeleteAsync<TEntity>(string id, IEntityHeader org) where TEntity : class, IIDEntity, INoSQLEntity, IKeyedEntity, IOwnedEntity;
        Task UpsertDocumentAsync<TEntity>(TEntity obj) where TEntity : class, IIDEntity, INoSQLEntity, IKeyedEntity, IOwnedEntity;
        void SetConnection(IConnectionSettings connectionSettings);
        Task<RatedEntity> AddRatingAsync(string id, int rating, EntityHeader org, EntityHeader user);
        Task<InvokeResult> SetCategoryAsync(string id, EntityHeader category, EntityHeader org, EntityHeader user);
    }
}
