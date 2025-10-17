// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 5f423dc232ab5640d2685c61cf5e90885a4da48473c57acd270ccf233b46849d
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Validation;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Middleware
{
    public interface ITransmitter
    {
        Task<InvokeResult> TransmitAsync(IMessage message);
    }
}
