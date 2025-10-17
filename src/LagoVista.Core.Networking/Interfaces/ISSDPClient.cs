// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 7029679fc9885f1f7a62fa02c344cbc378b7f1af77a1547be91ca7f0dfa95aa4
// IndexVersion: 1
// --- END CODE INDEX META ---
using System;
using System.Threading.Tasks;
using LagoVista.Core.Networking.Models;
using LagoVista.Core.PlatformSupport;

namespace LagoVista.Core.Networking.Interfaces
{
    public interface ISSDPClient
    {
        event EventHandler<uPnPDevice> NewDeviceFound;

        Task SsdpQueryAsync(string filter = "ssdp:all", int seconds = 5, int port = 1900);

        void Cancel();

        bool ShowDiagnostics { get; set; }

        ILogger Logger { get; set; }
    }
}
