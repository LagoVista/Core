
namespace LagoVista.Core.Rpc.Middleware
{
    public interface ITransceiver:  ITransmitter, IReceiver
    {
        bool IsRunning { get; }
    }
}
