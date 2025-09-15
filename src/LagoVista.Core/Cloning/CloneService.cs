using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using System;
using System.Reflection;
using LagoVista.Core.Attributes;
using System.Linq;
using System.Collections;
using System.Diagnostics;

namespace LagoVista.Core.Cloning
{
    public static class CloneService
    {
        public static TEntity Clone<TEntity>(TEntity entity, EntityHeader user, EntityHeader org, string name, string key) where TEntity : ICloneable<TEntity>, IOwnedEntity, IIDEntity, IKeyedEntity, INamedEntity, IAuditableEntity, new()
        {
            var originalId = entity.Id;
            var originalOwnerOrg = entity.OwnerOrganization == null ? null : EntityHeader.Create(entity.OwnerOrganization.Id, entity.OwnerOrganization.Text);
            var originalOwnerUser = entity.OriginalOwnerUser == null ? null : EntityHeader.Create(entity.OriginalOwnerUser.Id, entity.OriginalOwnerUser.Text);
            var originalCreatedBy = EntityHeader.Create(entity.CreatedBy.Id, entity.CreatedBy.Text);
            var originalCreationDate = entity.CreationDate;

            var cloned = new TEntity();

            CloneProperties(entity, cloned, user, org);

            cloned.Id = Guid.NewGuid().ToId();
            cloned.OwnerOrganization = org;
            cloned.CreatedBy = user;
            cloned.LastUpdatedBy = user;
            cloned.CreationDate = DateTime.UtcNow.ToJSONString();
            cloned.Name = name;
            cloned.Key = key;
            cloned.OriginalId = originalId;
            cloned.OriginalCreationDate = originalCreationDate;
            cloned.OriginallyCreatedBy = originalCreatedBy;
            cloned.OriginalOwnerOrganization = originalOwnerOrg;
            cloned.OriginalOwnerUser = originalOwnerUser;
            cloned.IsPublic = false;
            cloned.LastUpdatedDate = cloned.CreationDate;

            return cloned;
        }

        private static void CloneProperties(Object source, Object dest, EntityHeader user, EntityHeader org)
        {
            var properties = source.GetType().GetTypeInfo().GetAllProperties();
            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttributes(typeof(CloneOptionsAttribute), true).OfType<CloneOptionsAttribute>().FirstOrDefault();
                if (attr == null || attr.AutoClone)
                {
                    if (!prop.GetType().GetTypeInfo().ImplementedInterfaces.Contains(typeof(Interfaces.ICloneable)))
                    {
                        var value = prop.GetValue(source);
                        prop.SetValue(dest, value);
                        if (value != null)
                        {
                            UpdateProperty(value, user, org, 0);
                        }
                    }
                }
            }
        }

        private static void UpdateProperty(Object value, EntityHeader user, EntityHeader org, int level)
        {
            if(!(value is string))
            { 
                if (value.GetType().GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEnumerable)))
                {
                    var items = value as IEnumerable;
                    foreach (var item in items)
                    {
                        var itemChildProperties = item.GetType().GetTypeInfo().GetAllProperties();
                        foreach (var itemChildProp in itemChildProperties)
                        {
                            var attr = itemChildProp.GetCustomAttributes(typeof(CloneOptionsAttribute), true).OfType<CloneOptionsAttribute>().FirstOrDefault();
                            if (attr == null || attr.AutoClone)
                            {
                                var childValue = itemChildProp.GetValue(item);
                                UpdateProperty(childValue, user, org, level);
                            }
                        }
                    }
                }
                else
                {
                    var childProperties = value.GetType().GetTypeInfo().GetAllProperties();
                    foreach (var childProperty in childProperties)
                    {
                        if ((childProperty.CanRead && !childProperty.GetMethod.IsStatic) ||
                           (childProperty.CanWrite && !childProperty.SetMethod.IsStatic))
                        {
                            var attr = childProperty.GetCustomAttributes(typeof(CloneOptionsAttribute), true).OfType<CloneOptionsAttribute>().FirstOrDefault();
                            if (attr == null || attr.AutoClone)
                            {
                                var childValue = childProperty.GetValue(value);

                                if (childValue != null)
                                {
                                    UpdateProperty(childValue, user, org, level + 1);
                                }
                            }
                        }
                    }
                }
            }

            if (value is IIDEntity idEntity)
            {
                var origId = idEntity.Id;
                idEntity.Id = Guid.NewGuid().ToId();
                Debug.WriteLine("      ".PadLeft(level * 10 + 5) + origId + " " + idEntity.Id);
            }

            if (value is IOwnedEntity ownedEntity)
            {
                ownedEntity.IsPublic = false;
                ownedEntity.OwnerOrganization = org;
            }

            if (value is IAuditableEntity auditedEntity)
            {
                var now = DateTime.UtcNow.ToJSONString();
                auditedEntity.LastUpdatedBy = user;
                auditedEntity.CreatedBy = user;
                auditedEntity.LastUpdatedDate = now;
                auditedEntity.CreationDate = now;
            }
        }
    }
}
