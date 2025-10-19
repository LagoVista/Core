// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 91fb9e0e53bc4995fac1e911ec16d2474d7bbec6c28882e935c30fc9466394f6
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Validation;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.WiFi
{
    public interface IWiFiNetworksService
    {
        event EventHandler<WiFiNetworkSelectedEventArgs> SsidUpdated;

        event EventHandler<WiFiConnection> Connected;

        event EventHandler<WiFiConnection> Disconnected;

        ObservableCollection<WiFiConnection> AllConnections { get; }

        ObservableCollection<WiFiConnection> FilteredConnections { get; }

        ObservableCollection<String> SsidConnectionFilters { get; }

        Task StartAsync(WiFiAdapter adapter);

        Task StopAsync();

        Task<InvokeResult> ConnectAsync(WiFiConnection connection);

        Task<InvokeResult> DisconnectAsync(WiFiConnection connection);

        WiFiConnection CurrentConnection { get; }
    }
}
