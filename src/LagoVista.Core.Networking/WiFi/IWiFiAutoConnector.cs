using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.WiFi
{
    public interface IWiFiAutoConnector
    {
        Task<bool> CheckAuthroizationAsync();

        event EventHandler<WiFiConnection> Connected;
        event EventHandler<WiFiConnection> Disconnected;

        Task<InvokeResult> InitAsync(WiFiAdapter preferredAdapter);
        Task<WiFiAdapter[]> GetAdapterList();

        ObservableCollection<String> SsidConnectionFilters { get; }
    }
}
