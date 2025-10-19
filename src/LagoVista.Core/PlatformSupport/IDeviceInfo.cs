// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: fc3ac6c7694f56f235802b7a7ee98472faa5119a436b454c6e7a76b5d813354e
// IndexVersion: 0
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.PlatformSupport
{
    public interface IDeviceInfo
    {
        String DeviceUniqueId { get; }
        String DeviceType { get; }
    }
}
