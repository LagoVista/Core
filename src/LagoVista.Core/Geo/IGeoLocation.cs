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
