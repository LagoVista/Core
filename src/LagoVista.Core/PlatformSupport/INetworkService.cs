// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 486e19d1f20e8b9d48e06fa48832d10e6b7fa6c2c4d423cb8fbb9111cd55ee8b
// IndexVersion: 2
// --- END CODE INDEX META ---
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
