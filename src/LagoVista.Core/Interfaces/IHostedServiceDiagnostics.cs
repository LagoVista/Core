using LagoVista.Core.Models.Dignostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public interface IHostedServiceDiagnostics
    {
        string Name { get; }
        HostedServiceDiagnosticSnapshot GetSnapshot();
    }
}
