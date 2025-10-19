// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 6baec1f751c1b626871a5560e78de8a2650b2fe61568a87ea88f1158de36a1b2
// IndexVersion: 0
// --- END CODE INDEX META ---
namespace LagoVista.Core.Geo
{
    public interface IGeoLocation
    {
        double? Altitude { get; }
        double ? Heading { get; }
        string LastUpdated { get; }
        double? Latitude { get; }
        double? Longitude { get; }

        double? AccuracyMeters { get; }
        double? Hdop { get; }
        double? Vdop { get; }

        bool HasLocation { get; }

        int? NumberSatellites { get; }
    }
}
