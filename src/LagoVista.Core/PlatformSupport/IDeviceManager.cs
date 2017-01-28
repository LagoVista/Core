using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.PlatformSupport
{
    public interface IDeviceManager
    {
        Task<ObservableCollection<SerialPortInfo>> GetSerialPortsAsync();
    }
}
