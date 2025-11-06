using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Utils.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;

    namespace Nuviot.RagIndexing
    {
        // Uses RagVectorPayload (with ToDictionary) and RagVectorPayload.BuildPointId you already have.

        public sealed class QdrantUpsertBatch
        {
            public BatchBody batch { get; set; } = new BatchBody();

            public sealed class BatchBody
            {
                public object[] ids { get; set; }
                public float[][] vectors { get; set; }
                public Dictionary<string, object>[] payloads { get; set; }
            }
        }

        public static class QdrantBatchBuilder
        {
            /// <summary>
            /// Build a single upsert batch from items.
            /// </summary>
            public static QdrantUpsertBatch Build(IEnumerable<(string pointId, float[] vector, RagVectorPayload payload)> items)
            {
                if (items == null) throw new ArgumentNullException(nameof(items));

                var arr = items.ToArray();
                if (arr.Length == 0) throw new ArgumentException("No items to build", nameof(items));

                // Basic dimension check (all vectors same length)
                int dim = arr[0].vector != null ? arr[0].vector.Length : 0;
                for (int i = 0; i < arr.Length; i++)
                    if (arr[i].vector == null || arr[i].vector.Length != dim)
                        throw new ArgumentException("Inconsistent embedding dimensions in batch.");

                var ids = new object[arr.Length];
                var vecs = new float[arr.Length][];
                var payloads = new Dictionary<string, object>[arr.Length];

                for (int i = 0; i < arr.Length; i++)
                {
                    ids[i] = (object)arr[i].pointId;
                    vecs[i] = arr[i].vector;
                    payloads[i] = arr[i].payload != null ? arr[i].payload.ToDictionary() : new Dictionary<string, object>();
                }

                return new QdrantUpsertBatch
                {
                    batch = new QdrantUpsertBatch.BatchBody
                    {
                        ids = ids,
                        vectors = vecs,
                        payloads = payloads
                    }
                };
            }

            /// <summary>
            /// Split items into multiple batches of at most maxPerBatch points.
            /// </summary>
            public static IEnumerable<QdrantUpsertBatch> BuildSplit(
                IEnumerable<(string pointId, float[] vector, RagVectorPayload payload)> items,
                int maxPerBatch)
            {
                if (items == null) throw new ArgumentNullException(nameof(items));
                if (maxPerBatch <= 0) throw new ArgumentOutOfRangeException(nameof(maxPerBatch));

                var chunk = new List<(string pointId, float[] vector, RagVectorPayload payload)>(maxPerBatch);
                foreach (var it in items)
                {
                    chunk.Add(it);
                    if (chunk.Count >= maxPerBatch)
                    {
                        yield return Build(chunk);
                        chunk.Clear();
                    }
                }
                if (chunk.Count > 0) yield return Build(chunk);
            }

            /// <summary>
            /// Create an HttpRequestMessage (PUT /collections/{collection}/points) for a given batch.
            /// Uses Newtonsoft.Json if available; else System.Text.Json fallback string.
            /// </summary>
            public static HttpRequestMessage CreateUpsertRequest(string baseUrl, string collection, QdrantUpsertBatch batch)
            {
                if (string.IsNullOrWhiteSpace(baseUrl)) throw new ArgumentException("baseUrl required", nameof(baseUrl));
                if (string.IsNullOrWhiteSpace(collection)) throw new ArgumentException("collection required", nameof(collection));
                if (batch == null) throw new ArgumentNullException(nameof(batch));

                var url = baseUrl.TrimEnd('/') + "/collections/" + collection + "/points";

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(batch);
                var req = new HttpRequestMessage(HttpMethod.Put, url);
                req.Content = new StringContent(json, Encoding.UTF8, "application/json");
                return req;
            }
        }
    }
}
