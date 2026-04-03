
using LagoVista.Core.Models.Diagnostics;

namespace LagoVista.Core.Interfaces
{
    public interface IHostedServiceDiagnostics
    {
        string Name { get; }
        HostedServiceDiagnosticSnapshot GetSnapshot();
    }
}
