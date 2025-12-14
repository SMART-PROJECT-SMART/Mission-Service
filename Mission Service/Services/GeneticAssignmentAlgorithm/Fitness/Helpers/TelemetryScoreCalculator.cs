using Core.Common.Enums;
using Core.Models;
using Mission_Service.Extensions;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Fitness.Helpers;

public static class TelemetryScoreCalculator
{
    public static double CalculateAssignmentTelemetryScore(
        Dictionary<TelemetryFields, double> telemetryWeights,
        Dictionary<TelemetryFields, double> uavTelemetryData,
        Location missionLocation
    )
    {
        double nonLocationTelemetryScore = CalculateNonLocationTelemetryScore(
            telemetryWeights,
            uavTelemetryData
        );

        double locationBasedScore = CalculateLocationBasedScore(
            telemetryWeights,
            uavTelemetryData,
            missionLocation
        );

        return nonLocationTelemetryScore + locationBasedScore;
    }

    private static double CalculateNonLocationTelemetryScore(
        Dictionary<TelemetryFields, double> telemetryWeights,
        Dictionary<TelemetryFields, double> uavTelemetryData
    )
    {
        double telemetryScore = 0.0;

        foreach (KeyValuePair<TelemetryFields, double> telemetryWeight in telemetryWeights)
        {
            if (!LocationProximityCalculator.IsLocationField(telemetryWeight.Key))
            {
                telemetryScore += telemetryWeight.CalculateWeightedTelemetryScore(uavTelemetryData);
            }
        }

        return telemetryScore;
    }

    private static double CalculateLocationBasedScore(
        Dictionary<TelemetryFields, double> telemetryWeights,
        Dictionary<TelemetryFields, double> uavTelemetryData,
        Location missionLocation
    )
    {
        double totalLocationWeight = CalculateTotalLocationWeight(telemetryWeights);

        if (totalLocationWeight <= 0.0)
        {
            return 0.0;
        }

        Location uavLocation = LocationProximityCalculator.ExtractUAVLocation(uavTelemetryData);
        double proximityScore = LocationProximityCalculator.CalculateNormelizedProximityScore(
            uavLocation,
            missionLocation
        );

        return proximityScore * totalLocationWeight;
    }

    private static double CalculateTotalLocationWeight(
        Dictionary<TelemetryFields, double> telemetryWeights
    )
    {
        double totalLocationWeight = 0.0;

        foreach (KeyValuePair<TelemetryFields, double> telemetryWeight in telemetryWeights)
        {
            if (LocationProximityCalculator.IsLocationField(telemetryWeight.Key))
            {
                totalLocationWeight += telemetryWeight.Value;
            }
        }

        return totalLocationWeight;
    }
}
