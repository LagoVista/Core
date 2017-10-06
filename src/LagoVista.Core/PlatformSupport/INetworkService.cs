using LagoVista.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace LagoVista.Core.PlatformSupport
{
    public interface INetworkService
    {
        event EventHandler NetworkInformationChanged;

        bool IsInternetConnected { get; }

        string GetIPV4Address();

        Task RefreshAysnc();
        
        ObservableCollection<NetworkDetails> AllConnections { get; }

        Task<bool> TestConnectivityAsync();
    
        void OpenURI(Uri uri);
    }
}
