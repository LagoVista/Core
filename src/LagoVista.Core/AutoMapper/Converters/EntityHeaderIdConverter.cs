using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Models;
using System;

namespace LagoVista.Core.AutoMapper
{
    public sealed class EntityHeaderIdConverter : IMapValueConverter
    {
        public bool CanConvert(Type sourceType, Type targetType)
        {
            var st = Nullable.GetUnderlyingType(sourceType) ?? sourceType;
            var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (!IsEntityHeaderType(st))
                return false;

            // Allow common id target types
            return tt == typeof(string) || tt == typeof(Guid) || tt == typeof(int);
        }

        public object Convert(object sourceValue, Type targetType)
        {
            if (sourceValue == null)
                return null;

            // Works for both EntityHeader and EntityHeader<T>
            var id = GetEntityHeaderId(sourceValue);
            var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (tt == typeof(string))
                return id;

            if (tt == typeof(Guid))
            {
                if (String.IsNullOrWhiteSpace(id))
                {
                    if (Nullable.GetUnderlyingType(targetType) != null)
                        return null;

                    throw new InvalidOperationException("Cannot convert empty EntityHeader.Id to non-nullable Guid.");
                }

                Guid g;
                if (!Guid.TryParse(id, out g))
                    throw new InvalidOperationException("Could not convert EntityHeader.Id to Guid: '" + id + "'.");

                return g;
            }

            if (tt == typeof(int))
            {
                if (String.IsNullOrWhiteSpace(id))
                {
                    if (Nullable.GetUnderlyingType(targetType) != null)
                        return null;

                    throw new InvalidOperationException("Cannot convert empty EntityHeader.Id to non-nullable int.");
                }

                int i;
                if (!Int32.TryParse(id, out i))
                    throw new InvalidOperationException("Could not convert EntityHeader.Id to int: '" + id + "'.");

                return i;
            }

            throw new InvalidOperationException("Unsupported conversion from EntityHeader to " + targetType.Name + ".");
        }

        private static bool IsEntityHeaderType(Type type)
        {
            if (type == typeof(EntityHeader))
                return true;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(EntityHeader<>))
                return true;

            return false;
        }

        private static string GetEntityHeaderId(object entityHeader)
        {
            // EntityHeader and EntityHeader<T> both expose an Id property in LagoVista.Core.Models
            var idProp = entityHeader.GetType().GetProperty("Id");
            if (idProp == null || idProp.PropertyType != typeof(string))
                throw new InvalidOperationException("EntityHeader type '" + entityHeader.GetType().FullName + "' does not expose a string Id property.");

            return (string)idProp.GetValue(entityHeader);
        }
    } 
}
