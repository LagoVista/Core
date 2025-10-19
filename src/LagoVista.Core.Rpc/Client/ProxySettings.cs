// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 1d7615ff5583bce7575a2bfed21de27937106ef03ef60eb5087fc0cd2a048613
// IndexVersion: 0
// --- END CODE INDEX META ---
using System;

namespace LagoVista.Core.Rpc.Client
{
    public sealed class ProxySettings : IProxySettings
    {
        public string OrganizationId { get; set; }
        private string _instanceId;
        public string InstanceId
        {
            get => _instanceId;
            set
            {
                if (!string.IsNullOrEmpty(_hostId))
                {
                    throw new InvalidOperationException("Host Id has already been provided, can only provide HostId or InstanceId, but not both.");
                }
                _instanceId = value;
            }
        }

        private string _hostId;
        public string HostId
        {
            get => _hostId;
            set
            {
                if (!string.IsNullOrEmpty(_instanceId))
                {
                    throw new InvalidOperationException("Instance Id has already been provided, can only provide HostId or InstanceId, but not both.");
                }
                _hostId = value;
            }
        }
    }
}
