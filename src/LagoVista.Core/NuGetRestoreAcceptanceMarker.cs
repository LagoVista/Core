namespace LagoVista.Core
{
    /// <summary>
    /// Temporary public marker for F219 proving a downstream repository can
    /// restore and compile against a newly published Core package.
    /// This marker is not intended for master/source promotion.
    /// </summary>
    public static class NuGetRestoreAcceptanceMarker
    {
        public const string Revision = "F219-Core-to-CloudStorage-20261013";
    }
}
