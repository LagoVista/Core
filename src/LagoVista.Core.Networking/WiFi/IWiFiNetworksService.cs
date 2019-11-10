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
