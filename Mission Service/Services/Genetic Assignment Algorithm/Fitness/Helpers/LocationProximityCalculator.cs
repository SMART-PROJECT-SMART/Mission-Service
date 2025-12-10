using Core.Common.Enums;
using Core.Models;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Fitness.Helpers;

public static class LocationProximityCalculator
{
    public static bool IsLocationField(TelemetryFields field)
    {
        return field == TelemetryFields.Latitude
            || field == TelemetryFields.Longitude
            || field == TelemetryFields.Altitude;
    }

    public static Location ExtractUAVLocation(Dictionary<TelemetryFields, double> uavTelemetryData)
    {
        double latitude = uavTelemetryData.GetValueOrDefault(TelemetryFields.Latitude, 0.0);
        double longitude = uavTelemetryData.GetValueOrDefault(TelemetryFields.Longitude, 0.0);
        double altitude = uavTelemetryData.GetValueOrDefault(TelemetryFields.Altitude, 0.0);

        return new Location(latitude, longitude, altitude);
    }

    public static double CalculateProximityScore(Location uavLocation, Location missionLocation)
    {
        double latitudeDifference = uavLocation.Latitude - missionLocation.Latitude;
        double longitudeDifference = uavLocation.Longitude - missionLocation.Longitude;
        double altitudeDifference = uavLocation.Altitude - missionLocation.Altitude;

        double horizontalDistanceSquared =
            (latitudeDifference * latitudeDifference) + (longitudeDifference * longitudeDifference);
        double verticalDistanceSquared = altitudeDifference * altitudeDifference;

        double totalDistanceSquared = horizontalDistanceSquared + verticalDistanceSquared;
        double distance = Math.Sqrt(totalDistanceSquared);

        double normalizedProximity = 1.0 / (1.0 + distance);

        return normalizedProximity;
    }
}
