using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AutoMapper.Converters
{
    using global::LagoVista.Core.Interfaces.AutoMapper;
    using global::LagoVista.Core.Models;
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;

    public sealed class ToEntityHeaderConverter : IMapValueConverter
    {
        private static readonly ConcurrentDictionary<Type, MethodInfo?> _methodCache =
            new ConcurrentDictionary<Type, MethodInfo?>();

        public bool CanConvert(Type sourceType, Type targetType)
        {
            if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));
            if (targetType == null) throw new ArgumentNullException(nameof(targetType));

            var st = Nullable.GetUnderlyingType(sourceType) ?? sourceType;
            var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (tt != typeof(EntityHeader))
                return false;

            // Only convert if the source type has a public instance method:
            // EntityHeader ToEntityHeader()
            return GetToEntityHeaderMethod(st) != null;
        }

        public object Convert(object sourceValue, Type targetType)
        {
            if (sourceValue == null)
                return null;

            var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;
            if (tt != typeof(EntityHeader))
                throw new InvalidOperationException($"ToEntityHeaderConverter only supports converting to EntityHeader, not {targetType.Name}.");

            var st = sourceValue.GetType();
            var method = GetToEntityHeaderMethod(st);

            if (method == null)
                throw new InvalidOperationException($"Type '{st.FullName}' does not implement a compatible ToEntityHeader() method.");

            // Invoke: EntityHeader ToEntityHeader()
            var result = method.Invoke(sourceValue, parameters: null);

            if (result == null)
            {
                // Returning null is allowed only if targetType is nullable (EntityHeader is a ref type anyway)
                return null;
            }

            if (result is EntityHeader eh)
                return eh;

            // Defensive: should never happen given our checks.
            throw new InvalidOperationException(
                $"ToEntityHeader() on '{st.FullName}' returned '{result.GetType().FullName}', expected EntityHeader.");
        }

        private MethodInfo GetToEntityHeaderMethod(Type sourceType)
        {
            MethodInfo method;

            if (_methodCache.TryGetValue(sourceType, out method))
                return method;

            method = FindToEntityHeaderMethod(sourceType);

            // We cache even null results to avoid repeated reflection
            _methodCache[sourceType] = method;

            return method;
        }

        private MethodInfo FindToEntityHeaderMethod(Type sourceType)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            var method = sourceType.GetMethod(
                "ToEntityHeader",
                flags,
                null,
                Type.EmptyTypes,
                null);

            if (method == null)
                return null;

            if (method.ReturnType != typeof(EntityHeader))
                return null;

            return method;
        }
    }
}