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

        protected Task<DependentObjectCheckResult> CheckForDepenenciesAsync(Object instance)
        {
            return _dependencyManager.CheckForDependenciesAsync(instance);
        }

        protected async Task ConfirmNoDepenenciesAsync(Object instance)
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


        /// <summary>
        /// To be called when an object is renamed, will go through any objects that rely on this object and rename the entity header column.
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        protected Task RenameDependentObjectsAsync(object instance, string newName)
        {
            return _dependencyManager.RenameDependentObjectsAsync(instance, newName);
        }

        protected Task AuthorizeAsync(string userId, string orgId, string action, Object data = null)
        {
            return _security.AuthorizeAsync(userId, orgId, action, data);
        }

        protected Task AuthorizeAsync(EntityHeader userId, EntityHeader orgId, string action, Object data = null)
        {
            return _security.AuthorizeAsync(userId, orgId, action, data);
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
            return _security.LogEntityActionAsync(id, entityType, accessType, org, user);
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
