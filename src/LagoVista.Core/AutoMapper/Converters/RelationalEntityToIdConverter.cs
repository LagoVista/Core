using LagoVista.Core.Interfaces.AutoMapper;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.AutoMapper.Converters
{
    public class RelationalEntityToIdConverter : IMapValueConverter
    {
        public bool CanConvert(Type sourceType, Type targetType)
        {
            if (!typeof(RelationalEntityBase).IsAssignableFrom(sourceType))
                return false;

            if ((targetType == typeof(Guid) || targetType != typeof(Guid?) || targetType == typeof(string)))
                return true;

            return false;
        }

        public object Convert(object sourceValue, Type targetType)
        {
            if(sourceValue == null)
            {
                if (targetType == typeof(Guid?))
                    return null;
            
                if(targetType == typeof(string))
                    return null;

                throw new InvalidOperationException($"Unable to convert null value to {targetType.Name}");
            }

            var entity = sourceValue as RelationalEntityBase;

            if(targetType == typeof(Guid?) || targetType == typeof(Guid))
                return Guid.Parse(entity.Id);

            return entity.Id;
        }
    }
}
