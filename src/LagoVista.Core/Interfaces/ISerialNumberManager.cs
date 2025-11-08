// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 44b8c5fe9eab11c9a3c43908817e56c01975c9ca8b954b636cf1f68518ef3052
// IndexVersion: 2
// --- END CODE INDEX META ---
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
