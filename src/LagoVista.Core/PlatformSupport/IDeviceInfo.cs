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
