// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 1b6a7a738041194dd3d9bdd84c1566ef42d3c81b37980b0d5594f6bfe2d4aeed
// IndexVersion: 2
// --- END CODE INDEX META ---
namespace LagoVista.Core.Interfaces
{
    public interface IRevisionedEntity
    {
        int Revision { get; set; }
        string RevisionTimeStamp { get; set; }
    }
}
