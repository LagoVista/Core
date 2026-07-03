using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace LagoVista.Core.AI.Services
{
    public static class EntityRagText
    {
        public static string Normalize(string value)
        {
            return NormalizeInline(value);
        }

        public static string NormalizeInline(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var withoutTags = Regex.Replace(value, "<[^>]+>", " ");
            var decoded = WebUtility.HtmlDecode(withoutTags);

            return Regex.Replace(decoded, @"\s+", " ").Trim();
        }

        public static string NormalizeMarkdown(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var normalizedLineEndings = value.Replace("\r\n", "\n").Replace('\r', '\n');
            var withoutTags = Regex.Replace(normalizedLineEndings, "<[^>]+>", " ");
            var decoded = WebUtility.HtmlDecode(withoutTags);

            var lines = decoded
                .Split('\n')
                .Select(line => Regex.Replace(line, @"[ \t]+", " ").TrimEnd())
                .ToList();

            var builder = new StringBuilder();
            var previousLineWasEmpty = false;

            foreach (var line in lines)
            {
                var isEmpty = String.IsNullOrWhiteSpace(line);

                if (isEmpty)
                {
                    if (!previousLineWasEmpty && builder.Length > 0)
                    {
                        builder.AppendLine();
                    }

                    previousLineWasEmpty = true;
                    continue;
                }

                builder.AppendLine(line.Trim());
                previousLineWasEmpty = false;
            }

            return builder.ToString().Trim();
        }

        public static string NormalizeAndLimit(string value, int maxLength)
        {
            var normalized = NormalizeInline(value);

            return Limit(normalized, maxLength);
        }

        public static string NormalizeAndLimitMarkdown(string value, int maxLength)
        {
            var normalized = NormalizeMarkdown(value);

            return LimitMarkdown(normalized, maxLength);
        }

        public static string ComputeSha256(string value)
        {
            var normalized = NormalizeInline(value);

            if (String.IsNullOrWhiteSpace(normalized))
            {
                return null;
            }

            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(normalized);
            var hash = sha.ComputeHash(bytes);
            var builder = new StringBuilder(hash.Length * 2);

            foreach (var item in hash)
            {
                builder.Append(item.ToString("x2"));
            }

            return builder.ToString();
        }

        public static void AppendSection(StringBuilder builder, string heading, string content)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var normalized = NormalizeInline(content);

            if (String.IsNullOrWhiteSpace(normalized))
            {
                return;
            }

            if (!String.IsNullOrWhiteSpace(heading))
            {
                builder.AppendLine($"## {NormalizeInline(heading)}");
                builder.AppendLine();
            }

            builder.AppendLine(normalized);
            builder.AppendLine();
        }

        public static void AppendListSection(StringBuilder builder, string heading, IEnumerable<string> items, int? maxItems = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var normalizedItems = NormalizeList(items, maxItems);

            if (!normalizedItems.Any())
            {
                return;
            }

            if (!String.IsNullOrWhiteSpace(heading))
            {
                builder.AppendLine($"## {NormalizeInline(heading)}");
                builder.AppendLine();
            }

            foreach (var item in normalizedItems)
            {
                builder.AppendLine($"- {item}");
            }

            builder.AppendLine();
        }

        public static IReadOnlyList<string> NormalizeList(IEnumerable<string> items, int? maxItems = null)
        {
            if (items == null)
            {
                return Array.Empty<string>();
            }

            var normalized = items
                .Select(NormalizeInline)
                .Where(item => !String.IsNullOrWhiteSpace(item))
                .Distinct(StringComparer.OrdinalIgnoreCase);

            if (maxItems.HasValue && maxItems.Value > 0)
            {
                normalized = normalized.Take(maxItems.Value);
            }

            return normalized.ToList();
        }

        private static string Limit(string value, int maxLength)
        {
            if (String.IsNullOrWhiteSpace(value) || maxLength <= 0 || value.Length <= maxLength)
            {
                return value;
            }

            var candidate = value.Substring(0, maxLength).TrimEnd();
            var lastSpace = candidate.LastIndexOf(' ');

            if (lastSpace > maxLength * 0.75)
            {
                candidate = candidate.Substring(0, lastSpace).TrimEnd();
            }

            return candidate.TrimEnd('.', ',', ';', ':', '-') + "…";
        }

        private static string LimitMarkdown(string value, int maxLength)
        {
            if (String.IsNullOrWhiteSpace(value) || maxLength <= 0 || value.Length <= maxLength)
            {
                return value;
            }

            var lines = value.Split('\n');
            var builder = new StringBuilder();

            foreach (var line in lines)
            {
                var requiredLength = line.Length + (builder.Length > 0 ? 1 : 0);

                if (builder.Length + requiredLength > maxLength)
                {
                    break;
                }

                if (builder.Length > 0)
                {
                    builder.AppendLine();
                }

                builder.Append(line);
            }

            var result = builder.ToString().Trim();

            if (!String.IsNullOrWhiteSpace(result))
            {
                return result;
            }

            return Limit(value, maxLength);
        }
    }
}