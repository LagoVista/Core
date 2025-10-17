// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 265b5f83da29f6b4956f3856cc2071689728f94c341963712652a0de5b980ca6
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Validation;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Middleware
{
    public interface IReceiver
    {
        Task<InvokeResult> ReceiveAsync(IMessage message);
    }
}
