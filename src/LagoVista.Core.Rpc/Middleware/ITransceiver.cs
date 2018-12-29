
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
