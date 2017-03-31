using LagoVista.Core.Exceptions;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Resources;
using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static LagoVista.Core.Models.AuthorizeResult;

namespace LagoVista.Core.Managers
{
    public class ManagerBase
    {
        readonly ILogger _logger;
        readonly IAppConfig _appConfig;

        public ManagerBase(ILogger logger, IAppConfig appConfig)
        {
            _appConfig = appConfig;
            _logger = logger;
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
                throw new ValidationException(ValidationResource.Concurrency_Error, new System.Collections.Generic.List<Core.Validation.ValidationMessage>()
                {
                    new Core.Validation.ValidationMessage(
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
            if (!result.IsValid)
            {
                throw new ValidationException("Invalid Data", result.Errors);
            }
        }

        protected Task<AuthorizeResult> AuthorizeAsync(IOwnedEntity ownedEntity, AuthorizeActions action, EntityHeader user, EntityHeader org = null)
        {
            return Task.FromResult(AuthorizeResult.Authorized);
        }

        protected Task<DependentObjectCheckResult> CheckDepenenciesAsync(Object instance)
        {
            return Task.FromResult(new DependentObjectCheckResult()
            {

            });
        }

        protected Task<AuthorizeResult> AuthorizeOrgAccess(string userId, string orgId, Type entityType = null)
        {
            return Task.FromResult(AuthorizeResult.Authorized);
        }

        protected Task<AuthorizeResult> AuthorizeOrgAccess(EntityHeader user, string orgId, Type entityType = null)
        {
            return Task.FromResult(AuthorizeResult.Authorized);
        }

        protected Task<AuthorizeResult> AuthorizeOrgAccess(EntityHeader user, EntityHeader org, Type entityType = null)
        {
            return Task.FromResult(AuthorizeResult.Authorized);
        }

    }
}
