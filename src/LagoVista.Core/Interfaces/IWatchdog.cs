// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 609a892f9a61371ba98fbeb25c53ba8b981639c4cc34783d89ea6a23352f2e17
// IndexVersion: 1
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IWatchdog
    {
        event EventHandler Elapsed;

        TimeSpan Period { get; set; }
        void Enable();
        void Disable();
        void Feed();
    }
}
