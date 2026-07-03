using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LagoVista.Core.AI.Models.Rag
{
    public sealed class RagVectorPayload : RagVectorPayloadBase<RagVectorPayloadMeta, RagVectorPayloadExtra>
    {
        public override string ToString()
        {
            var contentType = !String.IsNullOrWhiteSpace(Meta.ContentType)
                ? Meta.ContentType
                : Meta.ContentTypeId != RagContentType.Unknown ? Meta.ContentTypeId.ToString() : "Unknown";

            return $"{Meta.OrgNamespace}/{Meta.ProjectId}/{Meta.DocId} ({contentType}) sec={Meta.SectionKey} p={Meta.PartIndex}/{Meta.PartTotal}";
        }

        public static RagVectorPayload FromDictionary(IDictionary<string, object> source)
        {
            return FromDictionary<RagVectorPayload>(source);
        }

        public static string BuildSemanticId(string docId, string sectionKey, int partIndex)
        {
            if (String.IsNullOrWhiteSpace(docId))
            {
                throw new ArgumentException("Document ID is required.", nameof(docId));
            }

            if (String.IsNullOrWhiteSpace(sectionKey))
            {
                sectionKey = "body";
            }

            if (partIndex < 1)
            {
                partIndex = 1;
            }

            return $"{docId}:sec:{Slug(sectionKey, "body")}#p{partIndex}";
        }

        public static string BuildPointId(string docId, string sectionKey, int partIndex)
        {
            return BuildSemanticId(docId, sectionKey, partIndex);
        }

        public static RagVectorPayload FromEntity(IEntityBase entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var entityType = entity.GetType();
            var attribute = entityType.GetTypeInfo().GetCustomAttributes<EntityDescriptionAttribute>().FirstOrDefault();
            var entityDescription = EntityDescription.Create(entityType, attribute);

            var payload = new RagVectorPayload
            {
                Meta =
                {
                    DocId = entity.Id,
                    Title = entity.Name,
                    SectionKey = "main",
                    BusinessDomainArea = "EntityModel",
                    BusinessDomainKey = entityDescription.DomainName,
                    SysDomain = "AppDomain",
                    SysLayer = "Server",
                    SysRole = "EntityModel",
                    PartIndex = 1,
                    PartTotal = 1,
                    Deleted = entity.IsDeleted ?? false,
                    SemanticId = $"{entityDescription.DomainName}:{entityType.Name}:{entity.Id}".Replace(" ", String.Empty).ToLowerInvariant(),
                    ContentTypeId = RagContentType.DomainDocument,
                    Subtype = entity.EntityType,
                    SubtypeFlavor = "ModelContents",
                    ProjectId = "default",
                    OrgId = entity.EntityType == "Organization" ? entity.Id.ToString() : entity.OwnerOrganization.Id
                }
            };

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

    }
}
