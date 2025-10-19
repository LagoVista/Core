// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: b374826b85d598a33edb303bb5b3a3eb395d096323c12501d5e0b25986574cf5
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Validation;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.WiFi
{
    public interface IWiFiAdaptersService
    {
        Task<ObservableCollection<WiFiAdapter>> GetAdapterListAsync();

        Task<InvokeResult> CheckAuthroizationAsync();
    }
}
