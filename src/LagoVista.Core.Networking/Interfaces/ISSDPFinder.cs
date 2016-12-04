using System;
using System.Threading.Tasks;
using LagoVista.Core.Networking.Models;

namespace LagoVista.Core.Networking.Interfaces
{
    public interface ISSDPFinder
    {
        event EventHandler<uPnPDevice> NewDeviceFound;

        Task SsdpQueryAsync(string filter = "ssdp:all", int seconds = 5);

        void Cancel();

        bool ShowDiagnostics { get; set; }
    }
}
