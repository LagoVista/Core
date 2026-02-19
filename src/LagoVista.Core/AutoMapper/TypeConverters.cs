using LagoVista.Core.Interfaces.AutoMapper;
using System;
using System.Collections.Generic;


namespace LagoVista.Core.AutoMapper
{
    public sealed class MapValueConverterRegistry : IMapValueConverterRegistry
    {
        private readonly List<IMapValueConverter> _converters;

        public MapValueConverterRegistry(IEnumerable<IMapValueConverter> converters)
        {
            _converters = converters == null ? new List<IMapValueConverter>() : new List<IMapValueConverter>(converters);
        }

        public bool TryConvert(object sourceValue, Type targetType, out object convertedValue)
        {
            convertedValue = null;

            if (sourceValue == null)
                return true; // treat null as convertible to anything nullable/ref type; mapper will set null

            var sourceType = sourceValue.GetType();

            for (var i = 0; i < _converters.Count; ++i)
            {
                if (_converters[i].CanConvert(sourceType, targetType))
                {
                    convertedValue = _converters[i].Convert(sourceValue, targetType);
                    return true;
                }
            }

            return false;
        }
    }
}
