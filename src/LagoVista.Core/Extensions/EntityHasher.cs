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
                $"/{nameof(IEntityBase.AISessions)}",
                $"/{nameof(IEntityBase.Sha256Hex)}",
                $"/{nameof(IEntityBase.PublicPromotedBy)}",
                $"/{nameof(IEntityBase.PublicPromotionDate)}",
                $"/_etag",
                $"/_rid",
                $"/_self",
                $"/_ts",
            };

        public static string CalculateHash(JToken token)
        {
            var canonical = NormalizeForHash(token, true);
            var json = canonical.ToString(Formatting.None);
            return Sha256Hex(json);
        }

        public static JToken NormalizeForHash(JToken token, bool dropNullsAndDefaults)
        {
            if (token == null) return JValue.CreateNull();

            // Work on a clone (no side effects)
            var working = token.DeepClone();

            // 1) remove volatile nodes
            foreach (var p in ExcludedNodes.Where(x => !string.IsNullOrWhiteSpace(x)))
                RemoveAtPointer(working, p);

            // 2) optionally drop nulls/defaults from raw JSON so it matches object serialization
            if (dropNullsAndDefaults)
                working = StripNullAndDefaultValues(working);

            // 3) sort object properties
            return Canonicalize(working);
        }


        private static JToken StripNullAndDefaultValues(JToken token)
        {
            if (token is JObject obj)
            {
                var props = obj.Properties().ToList();
                foreach (var p in props)
                {
                    var normalized = StripNullAndDefaultValues(p.Value);

                    // drop nulls
                    if (normalized.Type == JTokenType.Null)
                    {
                        p.Remove();
                        continue;
                    }

                    // drop "default" primitives (optional; only if you truly want to mimic DefaultValueHandling.Ignore)
                    if (normalized is JValue v)
                    {
                        if (v.Type == JTokenType.Boolean && v.Value is bool b && b == false)
                        {
                            // remove false booleans IF you consider them "default"
                            // Comment this out if false is meaningful in your domain
                            // p.Remove();
                        }
                    }

                    p.Value = normalized;
                }

                return obj;
            }

            if (token is JArray arr)
            {
                for (int i = 0; i < arr.Count; i++)
                    arr[i] = StripNullAndDefaultValues(arr[i]);

                return arr;
            }

            return token;
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
                DefaultValueHandling = DefaultValueHandling.Ignore,
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


        /// <summary>
        /// Canonicalizes a JToken by sorting all JObject properties (recursively).
        /// Arrays keep order (important: array order is semantic).
        /// </summary>
        private static JToken Canonicalize(JToken token)
        {
            if (token == null) return JValue.CreateNull();

            if (token is JObject obj)
            {
                var props = obj.Properties()
                    .OrderBy(p => p.Name, StringComparer.Ordinal)
                    .Select(p => new JProperty(p.Name, Canonicalize(p.Value)));

                var sorted = new JObject();
                foreach (var p in props) sorted.Add(p);
                return sorted;
            }

            if (token is JArray arr)
            {
                var newArr = new JArray();
                foreach (var item in arr)
                    newArr.Add(Canonicalize(item));
                return newArr;
            }

            // JValue, etc.
            return token.DeepClone();
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
