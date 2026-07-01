using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Utils
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public static class EntityChecklistHashUtils
    {
        public static string ComputeValueSha256(object value)
        {
            var canonicalValue = CreateCanonicalValue(value);

            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(canonicalValue);
            var hash = sha256.ComputeHash(bytes);

            return BitConverter.ToString(hash).Replace("-", String.Empty).ToLowerInvariant();
        }

        private static string CreateCanonicalValue(object value)
        {
            if (value == null)
            {
                return "null";
            }

            if (value is string stringValue)
            {
                return NormalizeString(stringValue);
            }

            var token = value as JToken ?? JToken.FromObject(value);
            var canonicalToken = CanonicalizeToken(token);

            return canonicalToken.ToString(Formatting.None);
        }

        private static JToken CanonicalizeToken(JToken token)
        {
            if (token is JObject obj)
            {
                var properties = obj.Properties()
                    .OrderBy(property => property.Name, StringComparer.Ordinal)
                    .Select(property => new JProperty(property.Name, CanonicalizeToken(property.Value)));

                return new JObject(properties);
            }

            if (token is JArray array)
            {
                return new JArray(array.Select(CanonicalizeToken));
            }

            return token.DeepClone();
        }

        private static string NormalizeString(string value)
        {
            return value.Trim().Replace("\r\n", "\n").Replace("\r", "\n");
        }
    }
}
