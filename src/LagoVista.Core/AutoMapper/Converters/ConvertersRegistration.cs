using LagoVista.Core.Interfaces;
using LagoVista.Core.Interfaces.AutoMapper;

namespace LagoVista.Core.AutoMapper.Converters
{
    [CriticalCoverage]
    public static class ConvertersRegistration
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMapValueConverter, ToEntityHeaderConverter>();
            services.AddSingleton<IMapValueConverter, EntityHeaderIdConverter>();
            services.AddSingleton<IMapValueConverter, RelationalEntityToIdConverter>();
            services.AddSingleton<IMapValueConverter, DateTimeIsoStringConverter>();
            services.AddSingleton<IMapValueConverter, DateOnlyStringConverter>();
            services.AddSingleton<IMapValueConverter, NumericStringConverter>();
            services.AddSingleton<IMapValueConverter, EntityHeaderEnumToStringConverter>();
            services.AddSingleton<IMapValueConverter, StringToEntityHeaderEnumConverter>();
            services.AddSingleton<IMapValueConverter, IdTranslateConverters>();
            services.AddSingleton<IMapValueConverter, EntityHeaderEnumToStringConverter>();
            services.AddSingleton<IMapValueConverterRegistry, MapValueConverterRegistry>();
        }

        public static IMapValueConverterRegistry DefaultConverterRegistery = new MapValueConverterRegistry(new IMapValueConverter[]
        {
            new RelationalEntityToIdConverter(),
            new ToEntityHeaderConverter(),
            new EntityHeaderIdConverter(),
            new DateTimeIsoStringConverter(),
            new DateOnlyStringConverter(),
            new NumericStringConverter(),
            new IdTranslateConverters(),
            new EntityHeaderTextConverter(),
            new EntityHeaderEnumToStringConverter(),
            new StringToEntityHeaderEnumConverter(),
        });

    }
}
