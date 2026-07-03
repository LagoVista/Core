using System;
using System.Text;

namespace LagoVista.Core.AI.Services
{
    public static class RagReferenceNameHelper
    {
        public static string ToReferenceType(string propertyName)
        {
            if (String.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException("Property name is required.", nameof(propertyName));
            }

            return ToKebabCase(propertyName);
        }

        public static string BuildPointId(string entityType, string entityId, string sourceField, int sourceIndex)
        {
            if (String.IsNullOrWhiteSpace(entityType))
            {
                throw new ArgumentException("Entity type is required.", nameof(entityType));
            }

            if (String.IsNullOrWhiteSpace(entityId))
            {
                throw new ArgumentException("Entity ID is required.", nameof(entityId));
            }

            if (String.IsNullOrWhiteSpace(sourceField))
            {
                throw new ArgumentException("Source field is required.", nameof(sourceField));
            }

            if (sourceIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sourceIndex));
            }

            return $"{ToKebabCase(entityType)}:{ToKebabCase(entityId)}:{ToKebabCase(sourceField)}:{sourceIndex}";
        }

        public static string ToKebabCase(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var builder = new StringBuilder();

            for (var index = 0; index < value.Length; index++)
            {
                var character = value[index];

                if (Char.IsUpper(character))
                {
                    if (builder.Length > 0 && builder[builder.Length - 1] != '-')
                    {
                        builder.Append('-');
                    }

                    builder.Append(Char.ToLowerInvariant(character));
                    continue;
                }

                if (Char.IsLetterOrDigit(character))
                {
                    builder.Append(Char.ToLowerInvariant(character));
                    continue;
                }

                if (builder.Length > 0 && builder[builder.Length - 1] != '-')
                {
                    builder.Append('-');
                }
            }

            return builder.ToString().Trim('-');
        }
    }
}