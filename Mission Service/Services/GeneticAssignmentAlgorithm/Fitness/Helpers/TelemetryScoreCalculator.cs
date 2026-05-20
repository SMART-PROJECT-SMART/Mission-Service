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
        return nonLocationTelemetryScore;
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

}
