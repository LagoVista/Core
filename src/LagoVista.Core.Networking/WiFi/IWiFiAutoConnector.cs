// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 932d839c4d3e9fd227b9b96a002c6c004f8a1f3a57362400fc2834cee8f1d58f
// IndexVersion: 2
// --- END CODE INDEX META ---
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
