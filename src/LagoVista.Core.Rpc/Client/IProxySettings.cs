namespace LagoVista.Core.Rpc.Client
{
    public interface IProxySettings
    {
        string OrganizationId { get; set; }
        string HostId { get; set; }
        string InstanceId { get; set; }
    }
}
