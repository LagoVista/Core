using LagoVista.Core.Interfaces.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;


namespace LagoVista.Core.AutoMapper
{
    public sealed class MapValueConverterRegistry : IMapValueConverterRegistry
    {
        private List<IMapValueConverter> _converters;

        public IEnumerable<IMapValueConverter> Converters => _converters;

        public MapValueConverterRegistry(IEnumerable<IMapValueConverter> converters)
        {
            _converters = converters == null ? new List<IMapValueConverter>() : new List<IMapValueConverter>(converters);
        }

        public bool TryConvert(object sourceValue, Type targetType, out object convertedValue)
        {
            convertedValue = null;

            if (sourceValue == null)
                return true;

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

        public bool CanConvert(Type sourceType, Type targetType)
        {
            if (sourceType == null) throw new ArgumentNullException(nameof(sourceType));
            if (targetType == null) throw new ArgumentNullException(nameof(targetType));

            var st = Nullable.GetUnderlyingType(sourceType) ?? sourceType;
            var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;

            for (var i = 0; i < _converters.Count; ++i)
                if (_converters[i].CanConvert(st, tt))
                    return true;

            return false;
        }

        /// <summary>
        /// Adds converters, skipping duplicates by converter Type. Returns the number added.
        /// </summary>
        public int AddRange(params IMapValueConverter[] converters)
            => AddRange((IEnumerable<IMapValueConverter>)converters);

        /// <summary>
        /// Adds converters, skipping duplicates by converter Type. Returns the number added.
        /// </summary>
        public int AddRange(IEnumerable<IMapValueConverter> converters)
        {
            if (converters == null) return 0;

            // Current types already registered.
            var existing = new HashSet<Type>(_converters.Select(c => c.GetType()));

            var added = 0;
            foreach (var c in converters)
            {
                if (c == null) continue;

                var ct = c.GetType();
                if (existing.Add(ct))
                {
                    _converters.Add(c);
                    added++;
                }
            }

            return added;
        }

        /// <summary>
        /// Optional helper: remove duplicates if someone constructed the registry with dup instances.
        /// Keeps first instance per converter Type.
        /// </summary>
        public void Deduplicate()
        {
            var seen = new HashSet<Type>();
            var distinct = new List<IMapValueConverter>(_converters.Count);

            for (var i = 0; i < _converters.Count; i++)
            {
                var c = _converters[i];
                if (c == null) continue;

                if (seen.Add(c.GetType()))
                    distinct.Add(c);
            }

            _converters.Clear();
            _converters.AddRange(distinct);
        }
    }
}
