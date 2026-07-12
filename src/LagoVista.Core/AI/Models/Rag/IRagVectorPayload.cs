using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models.Rag
{
    public interface IRagVectorPayload
    {
        string PointId { get; set; }

        IRagVectorPayloadMeta CoreMeta { get; }

        IRagVectorPayloadExtra CoreExtra { get; }

        IReadOnlyList<QdrantPayloadIndexSpec> GetIndexes();

        JObject Serialize();
    }

    public interface IRagVectorPayload<TMeta, TExtra> : IRagVectorPayload
        where TMeta : IRagVectorPayloadMeta
        where TExtra : IRagVectorPayloadExtra
    {

        TMeta Meta { get; }
        TExtra Extra { get; }
    }

    public interface IRagVectorPayloadMeta
    {
        string DocId { get; }

        string OrgId { get; }

        string Title { get; }

        RagContentType ContentTypeId { get; }

        string Subtype { get; }
    }

    public interface IRagVectorPayloadExtra
    {
        string ShortSummary { get; }
        string ModelContentUrl { get; }
    }
}
