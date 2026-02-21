using LagoVista.Core.Attributes;
using LagoVista.Core.Models;

namespace LagoVista.Core.Tests.Mapping
{
    public sealed partial class LagoVistaAutoMapperV1Tests
    {
        // ----------------------------
        // Test Models + Fakes
        // ----------------------------

        private sealed class PlainSource
        {
            public string name { get; set; }
            public string EXTERNALPROVIDERID { get; set; }
            public string ShouldNotCopy { get; set; }
        }

        private sealed class PlainEntityHeaderSource
        {
            [MapTo("Id")]
            [IgnoreOnMapTo]
            public EntityHeader<PlainSource> Source { get; set; }
        }

        private sealed class PlainEntityHeaderDestination
        {
            public string Id { get; set; }
        }

    }
}
