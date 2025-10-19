// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c6c8a5f3f273f582f2f8f212e9518c3ba0da85c937e23a37c93f2a225b1ad7e1
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Rpc.Settings;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Middleware
{
    public interface ITransceiver:  ITransmitter, IReceiver
    {
        Task StartAsync(ITransceiverConnectionSettings connectionSettings);

        Task RefreshConnection(ITransceiverConnectionSettings connectionSettings);

        bool IsRunning { get; }
    }
}
