// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 824e3047af5307cb7359f19e8cd0d7f141ada573f10c516a203e9564be98b6eb
// IndexVersion: 1
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.PlatformSupport
{
    public enum Locations
    {
        Default,
        Local,
        Roaming,
        Temp
    }

    public interface IStorageService
    {
        Task<string> StoreAsync(Stream stream, String fileName,Locations location = Locations.Default, String folder = "");
        Task<Stream> Get(Uri rui);
        Task<Stream> Get(String fileName, Locations location = Locations.Default, String folder = "");
        Task<T> GetKVPAsync<T>(String key, T defaultValue = default(T)) where T : class;
        Task StoreKVP<T>(String key, T value) where T : class;
        Task<bool> HasKVPAsync(String key);
        Task ClearKVP(String key);
        Task<string> StoreAsync<TObject>(TObject instance, string fileName) where TObject : class;
        Task<TObject> GetAsync<TObject>(string fileName) where TObject : class;
        Task<String> ReadAllTextAsync(String fileName);
        Task<string> WriteAllTextAsync(String fileName, string text);
        Task<List<string>> ReadAllLinesAsync(String fileName);
        Task<string> WriteAllLinesAsync(String fileName, List<string> text);
        Task<byte[]> ReadAllBytesAsync(String fileName);
        Task<string> WriteAllBytesAsync(String fileName, byte[] buffer);
        Task ClearAllAsync();
    }
}
    