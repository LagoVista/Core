using System;
using System.Threading.Tasks;
using LagoVista.Core.Networking.Models;
using LagoVista.Core.PlatformSupport;

namespace LagoVista.Core.Networking.Interfaces
{
    public interface ISSDPClient
    {
        event EventHandler<uPnPDevice> NewDeviceFound;

        Task SsdpQueryAsync(string filter = "ssdp:all", int seconds = 5);

        void Cancel();

        bool ShowDiagnostics { get; set; }

        ILogger Logger { get; set; }
    }
}
