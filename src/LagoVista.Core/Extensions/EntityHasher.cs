using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LagoVista
{
    public static class EntityHasher
    {
        public static List<string> ExcludedNodes  = new List<string>()
            {
                "/id",
                $"/{nameof(IEntityBase.OwnerOrganization)}",
                $"/{nameof(IEntityBase.OwnerUser)}",
                $"/{nameof(IEntityBase.CreatedBy)}",
                $"/{nameof(IEntityBase.LastUpdatedBy)}",
                $"/{nameof(IEntityBase.CreationDate)}",
                $"/{nameof(IEntityBase.LastUpdatedDate)}",
                $"/{nameof(IEntityBase.Revision)}",
                $"/{nameof(IEntityBase.RevisionTimeStamp)}",
                $"/{nameof(IEntityBase.AuditHistory)}",
                $"/{nameof(IEntityBase.ETag)}",
                $"/{nameof(IEntityBase.ClonedFromId)}",
                $"/{nameof(IEntityBase.ClonedFromOrg)}",
                $"/{nameof(IEntityBase.ClonedRevision)}",
                $"/{nameof(IEntityBase.DatabaseName)}",
                $"/{nameof(IEntityBase.AiEntitySessions)}",
                $"/AISessions",
                $"/{nameof(IEntityBase.Sha256Hex)}",
                $"/{nameof(IEntityBase.PublicPromotedBy)}",
                $"/{nameof(IEntityBase.DeletedBy)}",
                $"/{nameof(IEntityBase.DeletionDate)}",
                $"/{nameof(IEntityBase.DeprecatedBy)}",
                $"/{nameof(IEntityBase.DeprecationDate)}",
                $"/{nameof(IEntityBase.DeprecationDate)}",
                $"/{nameof(IEntityBase.ReadinessChecks)}",
                $"/{nameof(EntityBase.ReadinessReport)}",
                $"/{nameof(EntityBase.ReadinessStatus)}",
                $"/{nameof(EntityBase.ChecklistStatus)}",
                $"/{nameof(IEntityBase.DeprecationNotes)}",
                $"/{nameof(IEntityBase.PublicPromotionDate)}",
                $"/_etag",
                $"/_rid",
                $"/_self",
                $"/_ts",
                $"/_attachments",
            };

        public static JObject RemoveExcludedNodes(JObject source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var result = (JObject)source.DeepClone();

            foreach (var excludedNode in ExcludedNodes)
                RemoveNode(result, excludedNode);

            return result;
        }

        private static void RemoveNode(JObject root, string path)
        {
            if (root == null || String.IsNullOrWhiteSpace(path))
                return;

            var segments = path
                .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(UnescapeJsonPointerSegment)
                .ToArray();

            if (segments.Length == 0)
                return;

            JToken current = root;

            for (var index = 0; index < segments.Length - 1; index++)
            {
                if (current.Type != JTokenType.Object)
                    return;

                current = GetPropertyValue((JObject)current, segments[index]);

                if (current == null)
                    return;
            }

            if (current.Type != JTokenType.Object)
                return;

            var parent = (JObject)current;
            var property = GetProperty(parent, segments[segments.Length - 1]);

            property?.Remove();
        }

        private static JToken GetPropertyValue(JObject obj, string propertyName)
        {
            return GetProperty(obj, propertyName)?.Value;
        }

        private static JProperty GetProperty(JObject obj, string propertyName)
        {
            return obj
                .Properties()
                .FirstOrDefault(property =>
                    String.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase));
        }

        private static string UnescapeJsonPointerSegment(string segment)
        {
            return segment
                .Replace("~1", "/")
                .Replace("~0", "~");
        }

        public static string CalculateHash(JToken token)
        {
            var canonical = NormalizeForHash(token, true);
            var json = canonical.ToString(Formatting.None);
            return Sha256Hex(json);
        }



        public static void SetHash(this IEntityBase value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var settings = DefaultSettings();

            // 1) Serialize to JToken
            var token = JToken.FromObject(value, JsonSerializer.Create(settings));
            value.Sha256Hex = CalculateHash(token);
        }

        private static JsonSerializerSettings DefaultSettings()
        {
            return new JsonSerializerSettings
            {
                // Stable, minimal representation
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,

                // Avoid surprises
                ReferenceLoopHandling = ReferenceLoopHandling.Error
            };
        }

        private static string Sha256Hex(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                return ToHexLower(bytes);
            }
        }

        public static JToken NormalizeForHash(JToken token, bool dropNulls)
        {
            if (token == null) return JValue.CreateNull();

            var working = token.DeepClone();

            foreach (var p in ExcludedNodes.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                RemoveAtPointer(working, p);
            }

            if (dropNulls)
            {
                working = StripNullValues(working);
            }

            return Canonicalize(working);
        }

        private static JToken StripNullValues(JToken token)
        {
            if (token is JObject obj)
            {
                var props = obj.Properties().ToList();

                foreach (var p in props)
                {
                    var normalized = StripNullValues(p.Value);

                    if (normalized.Type == JTokenType.Null)
                    {
                        p.Remove();
                        continue;
                    }

                    p.Value = normalized;
                }

                return obj;
            }

            if (token is JArray arr)
            {
                for (var i = 0; i < arr.Count; i++)
                {
                    arr[i] = StripNullValues(arr[i]);
                }

                return arr;
            }

            return token;
        }


        /// <summary>
        /// Canonicalizes a JToken by sorting all JObject properties (recursively).
        /// Arrays keep order (important: array order is semantic).
        /// </summary>
        private static JToken Canonicalize(JToken token)
        {
            if (token == null) return JValue.CreateNull();

            if (token is JObject obj)
            {
                var normalizedProps = obj.Properties()
                    .Select(p => new
                    {
                        OriginalName = p.Name,
                        NormalizedName = ToCamelCase(p.Name),
                        Value = Canonicalize(p.Value)
                    })
                    .ToList();

                var duplicate = normalizedProps
                    .GroupBy(p => p.NormalizedName, StringComparer.Ordinal)
                    .FirstOrDefault(g => g.Count() > 1);

                if (duplicate != null)
                {
                    var names = string.Join(", ", duplicate.Select(p => p.OriginalName));
                    throw new InvalidOperationException($"JSON contains duplicate property names after camel-case normalization: {names}");
                }

                var sorted = new JObject();

                foreach (var p in normalizedProps.OrderBy(p => p.NormalizedName, StringComparer.Ordinal))
                {
                    sorted.Add(new JProperty(p.NormalizedName, p.Value));
                }

                return sorted;
            }

            if (token is JArray arr)
            {
                var newArr = new JArray();
                foreach (var item in arr)
                {
                    newArr.Add(Canonicalize(item));
                }

                return newArr;
            }

            return token.DeepClone();
        }

        private static string ToCamelCase(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return name;

            // Preserve Cosmos/system/meta fields like _etag, _rid, _self, _ts, _attachments, _t
            if (name[0] == '_') return name;

            // Already camelCase
            if (char.IsLower(name[0])) return name;

            // Simple PascalCase -> camelCase
            if (name.Length == 1) return name.ToLowerInvariant();

            return char.ToLowerInvariant(name[0]) + name.Substring(1);
        }

        /// <summary>
        /// Removes a token at a JSON Pointer-ish path, e.g. /a/b/0/c.
        /// Removes the property from its parent object or the element from its parent array.
        /// If the path doesn't exist, it's a no-op.
        /// </summary>
        public static void RemoveAtPointer(JToken root, string pointer)
        {
            if (root == null) return;

            var parts = SplitPointer(pointer);
            if (parts.Count == 0) return;

            // Traverse to parent
            JToken current = root;
            for (int i = 0; i < parts.Count - 1; i++)
            {
                current = Step(current, parts[i]);
                if (current == null) return;
            }

            var last = parts[parts.Count - 1];

            if (current is JObject parentObj)
            {
                var prop = parentObj.Properties()
                    .FirstOrDefault(p => string.Equals(p.Name, last, StringComparison.OrdinalIgnoreCase));

                prop?.Remove();
            }
            else if (current is JArray parentArr && int.TryParse(last, out var idx))
            {
                if (idx >= 0 && idx < parentArr.Count)
                    parentArr.RemoveAt(idx);
            }

        }

        private static string ToHexLower(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));

            var sb = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                sb.Append(bytes[i].ToString("x2")); // lowercase hex

            return sb.ToString();
        }

        private static JToken Step(JToken current, string segment)
        {
            var obj = current as JObject;
            if (obj != null)
            {
                foreach (var prop in obj.Properties())
                {
                    if (string.Equals(prop.Name, segment, StringComparison.OrdinalIgnoreCase))
                        return prop.Value;
                }
                return null;
            }

            var arr = current as JArray;
            if (arr != null)
            {
                int idx;
                if (!int.TryParse(segment, out idx)) return null;
                return (idx >= 0 && idx < arr.Count) ? arr[idx] : null;
            }

            return null;
        }


        private static List<string> SplitPointer(string pointer)
        {
            var p = pointer.Trim();
            if (p == "" || p == "/") return new List<string>();

            if (p.StartsWith("/")) p = p.Substring(1);

            // Minimal JSON Pointer decoding for "~1" and "~0"
            return p.Split('/')
                .Select(s => s.Replace("~1", "/").Replace("~0", "~"))
                .Where(s => s.Length > 0)
                .ToList();
        }
    }
}
