using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces.Crypto;
using LagoVista.Core.Models;
using System;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Services.Crypto
{
    /// <summary>
    /// Default implementation of modern v2 KeyId derivation.
    ///
    /// This type performs reflection-based resolution of IdPath (including dotted navigation paths).
    /// When navigation is missing and fallback configuration exists, it uses IKeyIdTargetResolver.
    ///
    /// Throws loudly when KeyId cannot be deterministically derived.
    /// </summary>
    public sealed class ModernKeyIdBuilder : IModernKeyIdBuilder
    {
        private readonly IKeyIdTargetResolver _resolver;

        public ModernKeyIdBuilder(IKeyIdTargetResolver resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        public async Task<string> BuildKeyGuiIdAsync<TDto>(TDto dto, ModernKeyIdAttribute attr, EntityHeader org, EntityHeader user, CancellationToken ct = default) where TDto : class
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (attr == null) throw new ArgumentNullException(nameof(attr));
            if (org == null) throw new ArgumentNullException(nameof(org));
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(attr.KeyIdFormat))
                throw new InvalidOperationException($"{attr.GetType().Name}.{nameof(attr.KeyIdFormat)} must be provided.");

            var orgId32 = CanonicalizeGuid32(org.Id, "org.Id");

            // Resolve {id}
            var resolvedIdGuid = TryResolveGuidFromPath(dto, attr.IdPath, out var idGuid);
            if (!resolvedIdGuid)
            {
                // Attempt fallback
                if (!string.IsNullOrWhiteSpace(attr.FallbackFkProperty))
                {
                    var fkGuid = ResolveIdProperty(dto, attr.FallbackFkProperty);
                    var targetPath = !string.IsNullOrWhiteSpace(attr.FallbackTargetPath)
                        ? attr.FallbackTargetPath
                        : attr.IdPath;

                    var g = await _resolver.ResolveIdAsync(targetPath, fkGuid, org, user, ct).ConfigureAwait(false);
                    idGuid = g.ToId();
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Modern KeyId derivation failed for DTO '{dto.GetType().Name}'. " +
                        $"Unable to resolve IdPath '{attr.IdPath}', and no fallback is configured.");
                }
            }

            var id32 = CanonicalizeGuid32(idGuid, $"{dto.GetType().Name}.{attr.IdPath}");

            var keyId = attr.KeyIdFormat
                .Replace("{id}", id32)
                .Replace("{orgId}", orgId32);

            if (string.IsNullOrWhiteSpace(keyId))
                throw new InvalidOperationException($"Modern KeyId format '{attr.KeyIdFormat}' produced an empty KeyId.");

            return keyId;
        }

        private static bool TryResolveGuidFromPath(object root, string path, out String  idField)
        {
            idField = default;

            if (string.IsNullOrWhiteSpace(path))
                return false;

            object current = root;
            Type currentType = root.GetType();

            var parts = path.Split('.');
            for (var i = 0; i < parts.Length; ++i)
            {
                var propName = parts[i];
                var prop = currentType.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
                if (prop == null)
                {
                    // configuration error
                    throw new InvalidOperationException(
                        $"Modern KeyId derivation misconfigured: type '{currentType.Name}' does not have a public instance property '{propName}' " +
                        $"(path '{path}').");
                }

                current = prop.GetValue(current);
                if (current == null)
                {
                    // navigation missing
                    return false;
                }

                currentType = current.GetType();
            }

            if (current is Guid g)
            {
                var guid = g;
                if (guid == Guid.Empty)
                    return false;

                idField = guid.ToId();
                return true;
            }

            // If the final value is a nullable Guid
            var underlying = Nullable.GetUnderlyingType(currentType);
            if (underlying == typeof(Guid))
            {
                // boxed nullable may come through as Guid already, but just in case
                if (current is Guid ng)
                {
                    var guid = ng;
                    if (guid == Guid.Empty)
                        return false;

                    idField = guid.ToId();
                    return true;
                }
            }


            if (current is NormalizedId32 n)
            {
                idField = n.ToString().ToLowerInvariant();
                return true;
            }

            if(current is string s)
            {
                if(Regex.Match(s, "^[A-F0-9]{32}$").Success)
                {
                    idField = s.Trim().ToLowerInvariant();
                    return true;
                }
                else if(String.IsNullOrEmpty(s))
                {
                    throw new InvalidOperationException($"Modern KeyId derivation misconfigured: path '{path}' value is null or empty.");
                }
                else
                {
                    throw new InvalidOperationException($"Modern KeyId derivation misconfigured: path '{path}' did not confirm to NormalizedGuid32 format value is {s}.");
                }
            }

            throw new InvalidOperationException(
                $"Modern KeyId derivation misconfigured: path '{path}' resolved to type '{currentType.Name}' but expected a String, Normalized32Guid or Guid.");
        }

        private static Guid ResolveIdProperty(object root, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            var prop = root.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (prop == null)
                throw new InvalidOperationException($"DTO type '{root.GetType().Name}' missing FK property '{propertyName}'.");

            var value = prop.GetValue(root);
            if (value == null)
                throw new InvalidOperationException($"FK property '{root.GetType().Name}.{propertyName}' resolved null.");

            if (value is Guid g)
            {
                if (g == Guid.Empty)
                    throw new InvalidOperationException($"FK property '{root.GetType().Name}.{propertyName}' is Guid.Empty.");
                return g;
            }

            throw new InvalidOperationException($"FK property '{root.GetType().Name}.{propertyName}' is type '{prop.PropertyType.Name}' but expected Guid.");
        }

        private static string CanonicalizeGuid32(string value, string label)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"{label} resolved empty; cannot build KeyId.");

            // value may already be 32-hex or may be a Guid string.
            // Prefer Guid parsing for correctness.
            if (Guid.TryParse(value, out var g))
                return g.ToString("N", CultureInfo.InvariantCulture).ToLowerInvariant();

            // accept already-canonical 32-hex
            var trimmed = value.Trim();
            if (trimmed.Length == 32)
                return trimmed.ToLowerInvariant();

            throw new InvalidOperationException($"{label} is not a valid Guid or 32-hex value. Value='{value}'.");
        }

        private static string CanonicalizeGuid32(Guid value, string label)
        {
            if (value == Guid.Empty)
                throw new InvalidOperationException($"{label} resolved Guid.Empty; cannot build KeyId.");

            return value.ToString("N", CultureInfo.InvariantCulture).ToLowerInvariant();
        }
    }
}
