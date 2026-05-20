using Core.Common.Enums;
using Core.Models;
using Mission_Service.Common.Constants;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Fitness.Helpers;

public static class LocationProximityCalculator
{
    public static bool IsLocationField(TelemetryFields field)
    {
        return field
            is TelemetryFields.Latitude
                or TelemetryFields.Longitude
                or TelemetryFields.Altitude;
    }

    public static Location ExtractUAVLocation(Dictionary<TelemetryFields, double> uavTelemetryData)
    {
        double latitude = uavTelemetryData.GetValueOrDefault(TelemetryFields.Latitude, 0.0);
        double longitude = uavTelemetryData.GetValueOrDefault(TelemetryFields.Longitude, 0.0);
        double altitude = uavTelemetryData.GetValueOrDefault(TelemetryFields.Altitude, 0.0);

        return new Location(latitude, longitude, altitude);
    }

    public static double CalculateNormelizedProximityScore(
        Location uavLocation,
        Location missionLocation
    )
    {
        double distanceKm = CalculateHorizontalDistanceKm(uavLocation, missionLocation);
        double normalizedProximity =
            MissionServiceConstants.GeoDistance.PROXIMITY_NUMERATOR
            / (MissionServiceConstants.GeoDistance.PROXIMITY_NUMERATOR + distanceKm);

        return normalizedProximity;
    }

    private static double CalculateHorizontalDistanceKm(Location source, Location target)
    {
        double sourceLatRad = DegreesToRadians(source.Latitude);
        double sourceLonRad = DegreesToRadians(source.Longitude);
        double targetLatRad = DegreesToRadians(target.Latitude);
        double targetLonRad = DegreesToRadians(target.Longitude);

        double deltaLat = targetLatRad - sourceLatRad;
        double deltaLon = targetLonRad - sourceLonRad;

        double haversine =
            Math.Sin(deltaLat / MissionServiceConstants.GeoDistance.HALF_DIVISOR)
                * Math.Sin(deltaLat / MissionServiceConstants.GeoDistance.HALF_DIVISOR)
            + Math.Cos(sourceLatRad)
                * Math.Cos(targetLatRad)
                * Math.Sin(deltaLon / MissionServiceConstants.GeoDistance.HALF_DIVISOR)
                * Math.Sin(deltaLon / MissionServiceConstants.GeoDistance.HALF_DIVISOR);

        double angularDistance =
            MissionServiceConstants.GeoDistance.FULL_ANGLE_MULTIPLIER
            * Math.Atan2(
                Math.Sqrt(haversine),
                Math.Sqrt(MissionServiceConstants.GeoDistance.PROXIMITY_NUMERATOR - haversine)
            );
        return MissionServiceConstants.GeoDistance.EARTH_RADIUS_KM * angularDistance;
    }

    private static double DegreesToRadians(double degrees) =>
        degrees * MissionServiceConstants.GeoDistance.DEGREES_TO_RADIANS_FACTOR;
}
