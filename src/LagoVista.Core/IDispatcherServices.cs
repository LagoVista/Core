// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 4352a41cb8f9d94ad687a31b178857feac4ea1189e0fac5d2af64da340d191ea
// IndexVersion: 0
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core
{
    public interface IDispatcherServices
    {
        void Invoke(Action action);
    }
}
