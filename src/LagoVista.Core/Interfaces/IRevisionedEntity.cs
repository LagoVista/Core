namespace LagoVista.Core.Interfaces
{
    public interface IRevisionedEntity
    {
        int Revision { get; set; }
        string RevisionTimeStamp { get; set; }
    }
}
