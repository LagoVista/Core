// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 09ae0ac8ba4a5060b7e881681b30211e68875b3a8125f83fcfcfa6369e2d8d15
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Networking.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Interfaces
{
    public interface IUDPSocket
    {
        event EventHandler<UDPMessageReceivedEventArgs> MessageReceived;

        Task BindEndpointAsync(String hostName, String serviceName);

        void JoinMulticastGroup(string hostName);

        Task<Stream> GetOutputStreamAsync(String hostName, int port);
    }
}
