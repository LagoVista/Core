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
