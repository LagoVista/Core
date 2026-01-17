using LagoVista.Core.Attributes;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace LagoVista
{
    public static class RagContentBuilder
    {
        private static readonly Regex _htmlTags = new Regex("<.*?>", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex _ws = new Regex(@"\s+", RegexOptions.Compiled);

        public static string StripHtml(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            var noTags = _htmlTags.Replace(input, " ");
            return _ws.Replace(noTags, " ").Trim();
        }

        public static string JoinLines(IEnumerable<string> lines)
        {
            return string.Join("\n", lines.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => l.Trim()));
        }

        public static string JoinBullets(IEnumerable<string> bullets)
        {
            return JoinLines(bullets.Where(b => !string.IsNullOrWhiteSpace(b)).Select(b => $"- {b.Trim()}"));
        }

        public static string Trunc(string s, int max)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;
            s = s.Trim();
            return s.Length <= max ? s : s.Substring(0, max).Trim();
        }

        public static bool IsMissing(string s, int minLen = 3)
        {
            if (string.IsNullOrWhiteSpace(s)) return true;
            var t = StripHtml(s);
            return t.Length < minLen ||
                   t.Equals("tbd", StringComparison.OrdinalIgnoreCase) ||
                   t.Equals("unknown", StringComparison.OrdinalIgnoreCase) ||
                   t.Equals("n/a", StringComparison.OrdinalIgnoreCase);
        }

        public static string EnumStableValue(Type owningEntityType, Enum enumValue)
        {
            if (enumValue == null) return null;

            var enumType = enumValue.GetType();
            var member = enumType.GetMember(enumValue.ToString()).FirstOrDefault();
            var enumLabel = member?.GetCustomAttribute<EnumLabelAttribute>();
            if (enumLabel == null) return enumValue.ToString().ToLowerInvariant();

            var constName = enumLabel.Key; // e.g. "Persona_RiskToleranceLevels_VeryHigh"
            var field = owningEntityType.GetField(constName, BindingFlags.Public | BindingFlags.Static);
            if (field?.FieldType == typeof(string))
                return (string)field.GetValue(null);

            return enumValue.ToString().ToLowerInvariant();
        }

        public static string DescribeEntityHeaderEnum<TEntity, TEnum>(EntityHeader<TEnum> hdr)
            where TEnum : struct
            where TEntity : class
        {
            if (hdr == null) return string.Empty;

            // Your EntityHeader<T> uses Values (per your note).
            // If your actual type uses Value, adjust accordingly.
            var valuesProp = hdr.GetType().GetProperty("Values");
            var raw = valuesProp?.GetValue(hdr);
            if (raw is Enum e)
            {
                var stable = EnumStableValue(typeof(TEntity), e);
                return $"{e} ({stable})";
            }

            // fallback
            if (!string.IsNullOrWhiteSpace(hdr.Text) && !string.IsNullOrWhiteSpace(hdr.Key))
                return $"{hdr.Text} ({hdr.Key})";

            return hdr.Text ?? hdr.Key ?? hdr.Id ?? string.Empty;
        }

        public static string DescribeEnum<TEntity>(Enum e)
        {
            if (e == null) return string.Empty;
            var stable = EnumStableValue(typeof(TEntity), e);
            return $"{e} ({stable})";
        }
    }
}
