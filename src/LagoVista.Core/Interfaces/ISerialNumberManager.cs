using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface ISerialNumberManager
    {
        Task<int> GenerateSerialNumber(string orgId, string key, string keyId = "NA", int seed = 1050);
    }
}
