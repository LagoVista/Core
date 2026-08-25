namespace LagoVista.Core.Interfaces
{
    /// <summary>
    /// Minimal record contract for compact, mutable operational data.
    /// Operational records represent the current working state used to run the platform
    /// rather than immutable activity history or document-oriented application data.
    /// </summary>
    public interface IOperationalRecord
    {
        string Id { get; set; }
        string OrganizationId { get; set; }
        string Organization { get; set; }
    }
}
