using Core.Common.Enums;
using Core.Models;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Fitness.Helpers;

public static class LocationProximityCalculator
{
    public static bool IsLocationField(TelemetryFields field)
    {
        return field is TelemetryFields.Latitude or TelemetryFields.Longitude or TelemetryFields.Altitude;
    }

    public static Location ExtractUAVLocation(Dictionary<TelemetryFields, double> uavTelemetryData)
    {
        double latitude = uavTelemetryData.GetValueOrDefault(TelemetryFields.Latitude, 0.0);
        double longitude = uavTelemetryData.GetValueOrDefault(TelemetryFields.Longitude, 0.0);
        double altitude = uavTelemetryData.GetValueOrDefault(TelemetryFields.Altitude, 0.0);

        return new Location(latitude, longitude, altitude);
    }

    public static double CalculateNormelizedProximityScore(Location uavLocation, Location missionLocation)
    {
        double distance = uavLocation.CalculateDistanceTo(missionLocation);
        double normalizedProximity = 1.0 / (1.0 + distance);

        return normalizedProximity;
    }
}
