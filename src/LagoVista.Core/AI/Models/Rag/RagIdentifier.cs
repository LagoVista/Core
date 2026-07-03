using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AI.Models.Rag
{
    internal static class RagIdentifier
    {
        public static string Slug(string value, string fallback)
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
    }
}
