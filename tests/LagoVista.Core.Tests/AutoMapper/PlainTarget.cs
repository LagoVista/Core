using LagoVista.Core.Attributes;


namespace LagoVista.Core.Tests.Mapping
{
    public sealed partial class LagoVistaAutoMapperV1Tests
    {
        private sealed class PlainTarget
        {
            public string Name { get; set; }

            [MapFrom(nameof(PlainSource.EXTERNALPROVIDERID))]
            public string ExternalProviderId { get; set; }

            [IgnoreOnMapTo]
            public string ShouldNotCopy { get; set; }
        }
    }
}
