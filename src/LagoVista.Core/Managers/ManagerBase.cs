// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 640c0951f4cfcf00a38113f1ddc28da6d371a24b3a0dd6a56d84ebe1512d072e
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Exceptions;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Resources;
using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static LagoVista.Core.Models.AuthorizeResult;

namespace LagoVista.Core.Managers
{
    public class ManagerBase
    {
        readonly ILogger _logger;
        readonly IAppConfig _appConfig;
        readonly IDependencyManager _dependencyManager;
        readonly ISecurity _security;

        public ManagerBase(ILogger logger, IAppConfig appConfig, IDependencyManager dependencyManager, ISecurity security)
        {
            _appConfig = appConfig;
            _logger = logger;
            _dependencyManager = dependencyManager;
            _security = security;
        }

        public IAppConfig AppConfig { get { return _appConfig; } }
        public ILogger Logger { get { return _logger; } }

        protected void SetCreatedBy(IAuditableEntity entity, IEntityHeader user)
        {
            var date = DateTime.Now.ToJSONString();
            entity.CreatedBy = EntityHeader.Create(user.Id, user.Text);
            entity.LastUpdatedBy = EntityHeader.Create(user.Id, user.Text);
            entity.CreationDate = date;
            entity.LastUpdatedDate = date;
        }

        protected void SetLastUpdated(IAuditableEntity entity, IEntityHeader user)
        {
            var date = DateTime.Now.ToJSONString();
            entity.LastUpdatedBy = EntityHeader.Create(user.Id, user.Text);
            entity.LastUpdatedDate = date;
        }

        protected void ConcurrencyCheck(IAuditableEntity fromRepo, string updatedDateStamp)
        {
            if (fromRepo.LastUpdatedDate != updatedDateStamp)
            {
                throw new ValidationException(ValidationResource.Concurrency_Error, new System.Collections.Generic.List<Core.Validation.ErrorMessage>()
                {
                    new Core.Validation.ErrorMessage(
                        ValidationResource.Concurrency_ErrorMessage
                            .Replace(Tokens.VALIDATION_USER_FULL_NAME, fromRepo.LastUpdatedBy.Text)
                            .Replace(Tokens.VALIDATION_DATESTAMP, fromRepo.LastUpdatedDate),
                        false)
                });
            }
        }

        protected void ValidationCheck(IValidateable entity, Actions action)
        {
            var entityBase = entity as EntityBase;
            if (entityBase != null && entityBase.IsDraft)
                return;

            var result = Validator.Validate(entity, action);
            if (!result.Successful)
            {
                throw new ValidationException("Invalid Data", result.Errors);
            }
        }



        public bool IsForInitialization { get; set; } = false;

        protected async Task AuthorizeAsync(IOwnedEntity ownedEntity, AuthorizeActions action, EntityHeader user, EntityHeader org, String actionName = null)
        {
            if (IsForInitialization)
                return;

            await _security.AuthorizeAsync(ownedEntity, action, user, org, actionName);
        }

        protected Task<DependentObjectCheckResult> CheckForDepenenciesAsync(IIDEntity instance)
        {
            return _dependencyManager.CheckForDependenciesAsync(instance);
        }

        protected async Task ConfirmNoDepenenciesAsync(IIDEntity instance)
        {
            var dependants = await _dependencyManager.CheckForDependenciesAsync(instance);
            if (dependants.IsInUse)
            {
                throw new InUseException(dependants);
            }
        }

        protected async Task AuthorizeOrgAccessAsync(string userId, string orgId, Type entityType = null, Actions action = Actions.Any, object data = null)
        {
            await _security.AuthorizeOrgAccessAsync(userId, orgId, entityType, action);
        }

        protected Task AuthorizeFinanceAdminAsync(EntityHeader user, EntityHeader org, string action, Object data = null)
        {
            return _security.AuthorizeFinanceAdminAsync(user, org, action, data);
        }

        protected Task AuthorizeAsync(string userId, string orgId, string action, Object data = null)
        {
            return _security.AuthorizeAsync(userId, orgId, action, data);
        }

        protected Task AuthorizeAsync(EntityHeader user, EntityHeader org, string action, Object data = null)
        {
            return _security.AuthorizeAsync(user, org, action, data);
        }

        protected Task AuthorizeAsync(EntityHeader user, EntityHeader org, Type entityType, Actions action)
        {
            return _security.AuthorizeAsync(user, org, entityType, action);
        }

        protected Task AuthorizeAsync(EntityHeader user, EntityHeader org, Type entityType, Actions action, string id)
        {
            return _security.AuthorizeAsync(user, org, entityType, action, id);
        }

        protected Task AuthorizeOrgAccessAsync(EntityHeader user, string orgId, Type entityType = null, Actions action = Actions.Any, object data = null)
        {
            return _security.AuthorizeOrgAccessAsync(user, orgId, entityType);
        }

        protected Task AuthorizeOrgAccessAsync(EntityHeader user, EntityHeader org, Type entityType = null, Actions action = Actions.Any, object data = null)
        {
            return _security.AuthorizeOrgAccessAsync(user, org, entityType);
        }

        protected Task LogEntityActionAsync(String id, string entityType, string accessType, EntityHeader org, EntityHeader user)
        {
            var kvps = new List<KeyValuePair<string, string>>();
            kvps.Add(entityType.ToKVP("entityType"));
            kvps.Add(accessType.ToKVP("accessType"));
            if(org != null)
            {
                kvps.Add(org.Id.ToKVP("orgId"));
                kvps.Add(org.Text.ToKVP("orgName"));
            }

            if(user != null)
            {
                user.Id.ToKVP("userId");
                user.Text.ToKVP("userName");
            }

            _logger.AddCustomEvent(LogLevel.Message, $"[{entityType}__{accessType}]", $"{entityType} Entity Access", kvps.ToArray()); 
              return _security.LogEntityActionAsync(id, entityType, accessType, org, user);
        }

        protected void AddAuditHistory(EntityBase entity, string fieldName, string oldvalue, string newValue, EntityHeader changedBy = null, string changedDate = null)
        {
            entity.AuditHistory.Add(new EntityChangeSet()
            {
                ChangeDate = changedDate ?? entity.LastUpdatedDate,
                ChangedBy = changedBy ?? entity.ToEntityHeader(),
                Changes = new List<EntityChange>()
                {
                    new EntityChange()
                    {
                        Field = fieldName,
                        OldValue = oldvalue,
                        NewValue = newValue
                    }
                }
            });
        }

        protected void ValidateAuthParams(EntityHeader org, EntityHeader user)
        {
            if(EntityHeader.IsNullOrEmpty(org))
            {
                throw new ValidationException("Org is Required for This Operation", new List<ErrorMessage>() { new ErrorMessage("Missing Org Entity Header") });
            }

            if (EntityHeader.IsNullOrEmpty(org))
            {
                throw new ValidationException("User is Required for This Operation", new List<ErrorMessage>() { new ErrorMessage("Missing User Entity Header") });
            }
        }
    }
}
