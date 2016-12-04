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
