namespace LagoVista.Core.Rpc.Client
{
    public sealed class ProxySettings: IProxySettings
    {
        public string OrganizationId { get; set; }
        public string InstanceId { get; set; }
    }
}
