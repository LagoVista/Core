using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace LagoVista.Core.Models
{
    public class EntityHeaderNode
    {
        public EntityHeaderNode(string path, JObject obj)
        {
            Path = path;
            Object = obj;
        }

        public string NormalizedPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Path))
                    return "/";

                var path = Path.Trim();

                if (path.StartsWith("$."))
                    path = path.Substring(2);
                else if (path.StartsWith("$"))
                    path = path.Substring(1);

                var segments = new List<string>();

                var current = "";
                for (int i = 0; i < path.Length; i++)
                {
                    var c = path[i];

                    if (c == '.')
                    {
                        if (current.Length > 0)
                        {
                            segments.Add(current);
                            current = "";
                        }
                    }
                    else if (c == '[')
                    {
                        if (current.Length > 0)
                        {
                            segments.Add(current);
                            current = "";
                        }

                        var end = path.IndexOf(']', i);
                        var index = path.Substring(i + 1, end - i - 1);
                        segments.Add(index);
                        i = end;
                    }
                    else
                    {
                        current += c;
                    }
                }

                if (current.Length > 0)
                    segments.Add(current);

                return "/" + string.Join("/", segments);
            }
        }

        /// <summary>
        /// Canonical path like /ownerOrganization or /devices/0/ownerOrganization
        /// </summary>
        public string Path { get; }

        /// <summary>The JObject that is the EntityHeader itself</summary>
        public JObject Object { get; }

        public string Id =>
            (Object["id"] ?? Object["Id"])?.Value<string>();

        public string Key =>
            (Object["key"] ?? Object["Key"])?.Value<string>();

        public string Text =>
            (Object["text"] ?? Object["Text"])?.Value<string>();

        public string EntityType =>
            (Object["entityType"] ?? Object["EntityType"])?.Value<string>();

        public string OwnerOrgId =>
            (Object["ownerOrgId"] ?? Object["OwnerOrgId"])?.Value<string>();

        public bool IsPublic =>
            (Object["isPublic"] ?? Object["IsPublic"])?.Value<bool>() ?? false;
   
    
        public bool? Resolved =>
            (Object[nameof(EntityHeader.Resolved).CamelCase()] ?? Object[nameof(EntityHeader.Resolved)])?.Value<bool?>();

    }
}
