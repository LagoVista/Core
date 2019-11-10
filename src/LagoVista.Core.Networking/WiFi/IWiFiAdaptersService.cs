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
