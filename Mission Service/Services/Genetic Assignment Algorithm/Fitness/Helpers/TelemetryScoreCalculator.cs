using Core.Common.Enums;
using Core.Models;
using Mission_Service.Extensions;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Fitness.Helpers;

public static class TelemetryScoreCalculator
{
    public static double CalculateAssignmentTelemetryScore(
        Dictionary<TelemetryFields, double> telemetryWeights,
        Dictionary<TelemetryFields, double> uavTelemetryData,
        Core.Models.Location missionLocation
    )
    {
        double telemetryScore = 0.0;
        double totalLocationWeight = 0.0;

        foreach (KeyValuePair<TelemetryFields, double> telemetryWeight in telemetryWeights)
        {
            if (LocationProximityCalculator.IsLocationField(telemetryWeight.Key))
            {
                totalLocationWeight += telemetryWeight.Value;
            }
            else
            {
                telemetryScore += telemetryWeight.CalculateWeightedTelemetryScore(uavTelemetryData);
            }
        }

        if (totalLocationWeight > 0.0)
        {
            Location uavLocation = LocationProximityCalculator.ExtractUAVLocation(
                uavTelemetryData
            );
            double proximityScore = LocationProximityCalculator.CalculateProximityScore(
                uavLocation,
                missionLocation
            );
            double weightedProximityScore = proximityScore * totalLocationWeight;
            telemetryScore += weightedProximityScore;
        }

        return telemetryScore;
    }
}
