using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LagoVista.Core.AI.Models.Rag
{
    public class RagEntityVectorPayload : RagVectorPayloadBase<RagEntityVectorPayloadMeta, RagEntityVectorPayloadExtra>
    {

        public static RagEntityVectorPayload FromEntity(IEntityBase entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var entityType = entity.GetType();
            var attribute = entityType.GetTypeInfo().GetCustomAttributes<EntityDescriptionAttribute>().FirstOrDefault();
            var entityDescription = EntityDescription.Create(entityType, attribute);

            if (entityDescription == null)
                throw new ArgumentNullException(nameof(entityDescription));

            var payload = new RagEntityVectorPayload
            {
                Meta =
                {
                    DocId = entity.Id,
                    Title = entity.Name,
                    EntityId = entity.Id,
                    EntityType = entityType.Name,
                    BusinessDomain = entityDescription.DomainName,
                    Deleted = entity.IsDeleted ?? false,
                    SemanticId = $"{entityDescription.DomainName}:{entityType.Name}:{entity.Id}".Replace(" ", String.Empty).ToLowerInvariant(),
                    ContentTypeId = RagContentType.DomainDocument,
                    Subtype = entity.EntityType,
                    SubtypeFlavor = "ModelContents",
                    OrgId = entity.EntityType == "Organization" ? entity.Id.ToString() : entity.OwnerOrganization.Id
                }
            };

            if (payload.Extra == null)
                throw new ArgumentNullException(nameof(payload.Extra));

            if (!String.IsNullOrEmpty(entityDescription.EditUIUrl))
            {
                payload.Extra.EditorUrl = entityDescription.EditUIUrl.Replace("{id}", entity.Id);
            }

            if (!String.IsNullOrEmpty(entityDescription.PreviewUIUrl))
            {
                payload.Extra.PreviewUrl = entityDescription.PreviewUIUrl.Replace("{id}", entity.Id);
            }

            if (!String.IsNullOrEmpty(entityDescription.GetUrl))
            {
                payload.Extra.RestGETUrl = entityDescription.GetUrl.Replace("{id}", entity.Id);
            }

            payload.Extra.RestPUTUrl = entityDescription.UpdateUrl;
            return payload;
        }

        public override JObject Serialize()
        {
            return JObject.FromObject(this);
        }
    }
}
