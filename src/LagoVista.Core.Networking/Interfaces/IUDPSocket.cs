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
