using LagoVista.Core.Models;
using LagoVista.Core.Models.EntityReadiness;
using System;
using System.Linq;

namespace LagoVista.Core.AI.Services
{
    public static class EntityRagReadinessExtensions
    {
        public static bool IsReadyFor(this IEntityBase entity, EntityReadinessCheckType checkType)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return entity.ReadinessChecks?.Any(item => item.CheckType == checkType && item.Status == EntityReadinessCheckStatus.Ready) == true;
        }

        public static string GetRagReadinessStage(this IEntityBase entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (entity.IsReadyFor(EntityReadinessCheckType.RelationshipReadiness))
            {
                return "relationship-ready";
            }

            if (entity.IsReadyFor(EntityReadinessCheckType.EntityDefinition))
            {
                return "entity-definition-ready";
            }

            if (entity.IsReadyFor(EntityReadinessCheckType.CoreDefinition))
            {
                return "core-definition-ready";
            }

            return "draft";
        }
    }
}