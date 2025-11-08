// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: e8fc5c09316bd5ddace9a3e67566d822d294bbf2c1bd2791744d840f2db1f657
// IndexVersion: 2
// --- END CODE INDEX META ---
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

        ISerialPort CreateSerialPort(SerialPortInfo portInfo);
    }
}
