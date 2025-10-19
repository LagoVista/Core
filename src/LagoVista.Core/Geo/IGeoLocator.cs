// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 84dc708ffa573ba8854245aaef3b525cdaaef5b002800fcef1394a3d4eabd914
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models.Geo;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Geo
{
    public interface IGeoLocator
    {
        Task InitAsync();

        event EventHandler<IGeoLocation> LocationUpdated;

        bool HasLocation { get; }

        bool HasLocationAccess { get; }

        IGeoLocation CurrentLocation { get; }
    }
}
