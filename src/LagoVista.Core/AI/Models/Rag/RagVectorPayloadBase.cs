using LagoVista.Core.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models.Rag
{
    public abstract class RagVectorPayloadBase<TMeta, TExtra> : IRagVectorPayload<TMeta, TExtra>
        where TMeta : RagCoreVectorPayloadMeta, IRagVectorPayloadMeta, new()
        where TExtra : RagCoreVectorPayloadExtra, IRagVectorPayloadExtra, new()
    {
        public const string BucketMeta = "Meta";
        public const string BucketExtra = "Extra";

        public string PointId { get; set; }

        protected RagVectorPayloadBase()
        {
            PointId = Guid.NewGuid().ToString().ToLower();
            Meta = new TMeta();
            Extra = new TExtra();
        }

        public TMeta Meta { get; set; }

        public TExtra Extra { get; set; }

        IRagVectorPayloadMeta IRagVectorPayload.CoreMeta => Meta;

        IRagVectorPayloadExtra IRagVectorPayload.CoreExtra => Extra;

        public InvokeResult ValidateForIndex()
        {
            var result = new InvokeResult();

            Meta.ValidateForIndex(result);
            Extra.ValidateForIndex(result);

            return result;
        }

        public Dictionary<string, object> ToDictionary()
        {
            return RagPayloadMapper.ToDictionary(Meta, Extra);
        }

        public QdrantPoint ToQdrantPoint(string pointId, float[] embedding)
        {
            if (String.IsNullOrWhiteSpace(pointId))
            {
                throw new ArgumentException("Point ID is required.", nameof(pointId));
            }

            if (embedding == null || embedding.Length == 0)
            {
                throw new ArgumentException("Embedding is required.", nameof(embedding));
            }

            return new QdrantPoint
            {
                Id = pointId,
                Vector = embedding,
                Payload = ToDictionary()
            };
        }

        public static IReadOnlyList<QdrantPayloadIndexSpec> Indexes => RagPayloadIndexResolver.GetIndexes<TMeta>(BucketMeta);

        public IReadOnlyList<QdrantPayloadIndexSpec> GetIndexes()
        {
            return RagPayloadIndexResolver.GetIndexes<TMeta>(BucketMeta);
        }

        protected static TPayload FromDictionary<TPayload>(IDictionary<string, object> source)
            where TPayload : RagVectorPayloadBase<TMeta, TExtra>, new()
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var metaSource = RagPayloadMapper.GetBucket(source, BucketMeta);
            var extraSource = RagPayloadMapper.GetBucket(source, BucketExtra);
            var flatFallback = metaSource == null && extraSource == null ? source : null;

            return new TPayload
            {
                Meta = RagPayloadMapper.FromBucket<TMeta>(metaSource ?? flatFallback),
                Extra = RagPayloadMapper.FromBucket<TExtra>(extraSource ?? flatFallback)
            };
        }

        protected static string Slug(string value, string fallback)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return fallback;
            }

            var builder = new StringBuilder(value.Length);
            var lower = value.Trim().ToLowerInvariant();

            foreach (var character in lower)
            {
                if ((character >= 'a' && character <= 'z') || (character >= '0' && character <= '9'))
                {
                    builder.Append(character);
                    continue;
                }

                if (Char.IsWhiteSpace(character) || character == '-' || character == '_' || character == '.' || character == '/')
                {
                    if (builder.Length > 0 && builder[builder.Length - 1] != '-')
                    {
                        builder.Append('-');
                    }
                }
            }

            var slug = builder.ToString().Trim('-');
            return String.IsNullOrWhiteSpace(slug) ? fallback : slug;
        }

        public abstract JObject Serialize();

    }
}