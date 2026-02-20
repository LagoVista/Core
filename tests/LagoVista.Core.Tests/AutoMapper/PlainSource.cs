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
    }
}
