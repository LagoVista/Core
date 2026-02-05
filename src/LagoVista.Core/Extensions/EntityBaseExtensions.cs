using LagoVista.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace LagoVista
{

    public static class EntityHeaderJson
    {
        // "NORMALIZEDGUID" => Guid with only letters+numbers (no dashes). Usually 32 chars.
        private static readonly Regex NormalizedGuidRegex =
            new Regex("^[A-Fa-f0-9]{32}$", RegexOptions.Compiled);

        /// <summary>
        /// Represents a discovered EntityHeader node in a JSON document.
        /// </summary>
       
        /// <summary>
        /// 1) Identify where EntityHeaders live by scanning the whole JSON token tree.
        /// Returns all matching nodes (path + JObject reference you can modify in-place).
        /// </summary>
        public static IReadOnlyList<EntityHeaderNode> FindEntityHeaderNodes(this IEntityBase entity)
        {
            var root = JToken.FromObject(entity);
            return FindEntityHeaderNodes(root);
        }

        public static IReadOnlyList<EntityHeaderNode> FindEntityHeaderNodes(string json)
        {
            var root = JToken.Parse(json);
            return FindEntityHeaderNodes(root);
        }

        public static IReadOnlyList<EntityHeaderNode> FindEntityHeaderNodes(JToken root)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));

            var results = new List<EntityHeaderNode>();
            Walk(root, "$", results);
            return results;

            static void Walk(JToken token, string path, List<EntityHeaderNode> results)
            {
                switch (token.Type)
                {
                    case JTokenType.Object:
                        {
                            var obj = (JObject)token;

                            if (LooksLikeEntityHeader(obj))
                            {
                                results.Add(new EntityHeaderNode(path, obj));
                                // Note: We still walk children because nested headers are possible, but rare.
                            }

                            foreach (var prop in obj.Properties())
                            {
                                var childPath = $"{path}.{prop.Name}";
                                Walk(prop.Value, childPath, results);
                            }

                            break;
                        }
                    case JTokenType.Array:
                        {
                            var arr = (JArray)token;
                            for (var i = 0; i < arr.Count; i++)
                            {
                                Walk(arr[i], $"{path}[{i}]", results);
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
        }



        /// <summary>
        /// Extract values from a found EntityHeader node.
        /// </summary>
        public static (string id, string key, string name, string entityType) Extract(EntityHeaderNode header)
        {
            if (header == null) throw new ArgumentNullException(nameof(header));

            var id = header.Object["Id"]?.Value<string>();
            var key = header.Object["Key"]?.Value<string>();
            var name = header.Object["Text"]?.Value<string>(); // NAME always present per your rule
            var entityType = header.Object["EntityType"]?.Value<string>();

            return (id, key, name, entityType);
        }

        /// <summary>
        /// 2) Update the node in-place. This modifies the original JSON tree since JObject is a reference.
        /// - key/entityType are optional; pass null to remove them (or choose not to remove via options)
        /// - name maps to "text" and should usually be provided
        /// </summary>
        public static void Update(
            this EntityBase entity,
            EntityHeaderNode header,
            string key = null,
            string name = null,
            string entityType = null,
            UpdateOptions options = null)
        {
            if (header == null) throw new ArgumentNullException(nameof(header));
            options ??= UpdateOptions.Default;

            Update(header.Object, key, name, entityType, options);
        }

        /// <summary>
        /// Overload if you already have the JObject for the EntityHeader.
        /// </summary>
        public static void Update(
            JObject entityHeaderObject,
            string key = null,
            string name = null,
            string entityType = null,
            UpdateOptions options = null)
        {
            if (entityHeaderObject == null) throw new ArgumentNullException(nameof(entityHeaderObject));
            options ??= UpdateOptions.Default;

            // Always update NAME/text if provided
            if (!string.IsNullOrWhiteSpace(name))
            {
                entityHeaderObject["Text"] = name.Trim();
            }

            // KEY: may or may not be present. You choose behavior.
            ApplyOptionalString(entityHeaderObject, "Key", key, options);

            // ENTITYTYPE: usually absent. You choose behavior.
            ApplyOptionalString(entityHeaderObject, "EntityType", entityType, options);
        }

        public sealed class UpdateOptions
        {
            /// <summary>If true and key/entityType is null/empty, remove the property if present.</summary>
            public bool RemoveWhenNullOrEmpty { get; set; } = false;

            /// <summary>If true, throws when name is null/empty.</summary>
            public bool RequireName { get; set; } = true;

            public static UpdateOptions Default => new UpdateOptions();
        }

        private static void ApplyOptionalString(JObject obj, string propName, string value, UpdateOptions options)
        {
            var hasValue = !string.IsNullOrWhiteSpace(value);

            if (hasValue)
            {
                obj[propName] = value.Trim();
                return;
            }

            if (options.RemoveWhenNullOrEmpty)
            {
                obj.Remove(propName);
            }
            // else: leave as-is
        }

        private static bool LooksLikeEntityHeader(JObject obj)
        {
            // Must have id and text per your rule.
            var id = obj["Id"]?.Type == JTokenType.String ? obj["Id"]!.Value<string>() : null;
            var text = obj["Text"]?.Type == JTokenType.String ? obj["Text"]!.Value<string>() : null;

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(text))
                return false;

            // id must be normalized guid-like: letters+numbers only.
            if (!NormalizedGuidRegex.IsMatch(id.Trim()))
                return false;

            // Optional: if key/entityType exist, they must be strings.
            if (obj.TryGetValue("Key", out var keyTok) && keyTok.Type != JTokenType.String && keyTok.Type != JTokenType.Null)
                return false;

            if (obj.TryGetValue("EntityType", out var etTok) && etTok.Type != JTokenType.String && etTok.Type != JTokenType.Null)
                return false;

            return true;
        }

        // -------------------------------------------------------
        // Public API
        // -------------------------------------------------------

        /// <summary>
        /// Updates BOTH:
        ///  1) the JSON EntityHeader JObject (in-place), and
        ///  2) the C# object graph at the same path (in-place),
        /// using the EntityHeaderNode.Path (like $.ownerUser or $.devices[0].ownerOrganization).
        /// </summary>
        public static void UpdateEntityHeaders(
            this IEntityBase rootClrObject,
            EntityHeaderNode headerNode,
            string key = null,
            string name = null,
            string entityType = null,
            EntityHeaderJson.UpdateOptions options = null)
        {
            if (rootClrObject == null) throw new ArgumentNullException(nameof(rootClrObject));
            if (headerNode == null) throw new ArgumentNullException(nameof(headerNode));


            // 2) Update CLR object at the same path
            var targetClrHeader = ResolveClrObjectAtPath(rootClrObject, headerNode.Path);
            if (targetClrHeader == null) return; // nothing to update, path didn't resolve

            UpdateEntityHeaderClr(targetClrHeader, key, name, entityType, options);
        }

        /// <summary>
        /// If you already resolved the CLR EntityHeader instance (or similar shape), update it in-place.
        /// Supports LagoVista-style: Id, Key, Text, EntityType (or Name/Text mapping).
        /// </summary>
        public static void UpdateEntityHeaderClr(
            object entityHeaderClr,
            string key = null,
            string name = null,
            string entityType = null,
            EntityHeaderJson.UpdateOptions options = null)
        {
            if (entityHeaderClr == null) throw new ArgumentNullException(nameof(entityHeaderClr));
            options ??= EntityHeaderJson.UpdateOptions.Default;

            // NAME: in JSON it is "text". In C# you might have Text, Name, or something else.
            // We'll try: Text, then Name.
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (!TrySetString(entityHeaderClr, "Text", name.Trim()) &&
                    !TrySetString(entityHeaderClr, "Name", name.Trim()))
                {
                    // If you have a different property name, add it here.
                }
            }

            // KEY: optional
            ApplyOptional(entityHeaderClr, "Key", key, options);

            // ENTITYTYPE: usually absent
            // Try EntityType first; if your CLR uses a different name, add it here.
            ApplyOptional(entityHeaderClr, "EntityType", entityType, options);
        }

        // -------------------------------------------------------
        // Path resolution into CLR object graph
        // -------------------------------------------------------

        /// <summary>
        /// Resolves a CLR object at a JSONPath-ish string like:
        ///   $.ownerUser
        ///   $.devices[0].ownerOrganization
        /// Returns the object instance at that path or null if unresolved.
        /// </summary>
        public static object ResolveClrObjectAtPath(object root, string jsonPath)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));
            if (string.IsNullOrWhiteSpace(jsonPath)) throw new ArgumentNullException(nameof(jsonPath));

            var segments = ParsePath(jsonPath);
            object current = root;

            foreach (var seg in segments)
            {
                if (current == null) return null;

                if (seg.Kind == PathSegmentKind.Property)
                {
                    current = GetMemberValue(current, seg.PropertyName);
                }
                else if (seg.Kind == PathSegmentKind.Index)
                {
                    current = GetIndexedValue(current, seg.Index);
                }
                else
                {
                    return null;
                }
            }

            return current;
        }

        private enum PathSegmentKind { Property, Index }

        private readonly struct PathSegment
        {
            public PathSegment(PathSegmentKind kind, string propName, int index)
            {
                Kind = kind;
                PropertyName = propName;
                Index = index;
            }

            public PathSegmentKind Kind { get; }
            public string PropertyName { get; }
            public int Index { get; }

            public static PathSegment Property(string name) => new PathSegment(PathSegmentKind.Property, name, -1);
            public static PathSegment Indexer(int i) => new PathSegment(PathSegmentKind.Index, null, i);
        }

        /// <summary>
        /// Parses "$.devices[0].ownerOrganization" into ["devices", [0], "ownerOrganization"] segments.
        /// </summary>
        private static List<PathSegment> ParsePath(string path)
        {
            // We expect the format produced by EntityHeaderJson: "$" then ".prop" and "[i]" segments.
            // Example: $.devices[0].ownerOrganization
            var p = path.Trim();
            if (p == "$") return new List<PathSegment>();

            if (p.StartsWith("$."))
                p = p.Substring(2);
            else if (p.StartsWith("$"))
                p = p.Substring(1);

            var segments = new List<PathSegment>();

            // Split by '.' but keep indexers inside tokens.
            var tokens = p.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var token in tokens)
            {
                // token can be "devices[0][1]" or "ownerOrganization"
                var cursor = 0;

                // read property name first (up to '[' or end)
                var bracket = token.IndexOf('[', cursor);
                var propName = bracket < 0 ? token : token.Substring(0, bracket);

                if (!string.IsNullOrWhiteSpace(propName))
                    segments.Add(PathSegment.Property(propName));

                cursor = propName.Length;

                // read any [n] indexers
                while (cursor < token.Length && token[cursor] == '[')
                {
                    var end = token.IndexOf(']', cursor);
                    if (end < 0) throw new FormatException($"Invalid path token: '{token}'");

                    var inner = token.Substring(cursor + 1, end - cursor - 1);
                    if (!int.TryParse(inner, out var idx))
                        throw new FormatException($"Invalid array index '{inner}' in path token: '{token}'");

                    segments.Add(PathSegment.Indexer(idx));
                    cursor = end + 1;
                }
            }

            return segments;
        }

        // -------------------------------------------------------
        // Reflection helpers
        // -------------------------------------------------------

        private static object GetMemberValue(object instance, string memberName)
        {
            if (instance == null) return null;
            if (string.IsNullOrWhiteSpace(memberName)) return null;

            var t = instance.GetType();

            // Property (public instance)
            var prop = t.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (prop != null && prop.CanRead)
                return prop.GetValue(instance);

            // Field (public instance)
            var field = t.GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (field != null)
                return field.GetValue(instance);

            return null;
        }

        private static object GetIndexedValue(object instance, int index)
        {
            if (instance == null) return null;
            if (index < 0) return null;

            // IList (List<T>, arrays via IList, ObservableCollection<T>, etc.)
            if (instance is IList list)
            {
                if (index >= list.Count) return null;
                return list[index];
            }

            // IEnumerable fallback (less ideal, but works for many collection types)
            if (instance is IEnumerable enumerable)
            {
                var i = 0;
                foreach (var item in enumerable)
                {
                    if (i == index) return item;
                    i++;
                }
            }

            return null;
        }

        private static bool TrySetString(object instance, string memberName, string value)
        {
            if (instance == null) return false;
            if (string.IsNullOrWhiteSpace(memberName)) return false;

            var t = instance.GetType();

            var prop = t.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (prop != null && prop.CanWrite && prop.PropertyType == typeof(string))
            {
                prop.SetValue(instance, value);
                return true;
            }

            var field = t.GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (field != null && field.FieldType == typeof(string))
            {
                field.SetValue(instance, value);
                return true;
            }

            return false;
        }

        private static void ApplyOptional(object instance, string memberName, string value, EntityHeaderJson.UpdateOptions options)
        {
            var hasValue = !string.IsNullOrWhiteSpace(value);

            if (hasValue)
            {
                TrySetString(instance, memberName, value.Trim());
                return;
            }

            if (options.RemoveWhenNullOrEmpty)
            {
                // CLR objects generally don't "remove" properties; set to null instead.
                TrySetString(instance, memberName, null);
            }
            // else leave as-is
        }
    }
}
