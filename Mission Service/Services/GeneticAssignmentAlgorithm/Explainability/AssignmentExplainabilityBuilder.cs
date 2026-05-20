using Core.Common.Enums;
using Core.Models;
using Microsoft.Extensions.Options;
using Mission_Service.Common.Constants;
using Mission_Service.Config;
using Mission_Service.Models;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Explainability.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Fitness.Helpers;
using Mission_Service.Services.UAVStatusService.Interfaces;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Explainability;

public class AssignmentExplainabilityBuilder : IAssignmentExplainabilityBuilder
{
    private readonly TelemetryWeightsConfiguration _telemetryWeights;
    private readonly FitnessWeightsConfiguration _fitnessWeights;
    private readonly IUAVStatusService _uavStatusService;

    public AssignmentExplainabilityBuilder(
        IOptions<TelemetryWeightsConfiguration> telemetryWeights,
        IOptions<FitnessWeightsConfiguration> fitnessWeights,
        IUAVStatusService uavStatusService
    )
    {
        _telemetryWeights = telemetryWeights.Value;
        _fitnessWeights = fitnessWeights.Value;
        _uavStatusService = uavStatusService;
    }

    public AssignmentExplainability Build(
        List<AssignmentGene> assignments,
        IReadOnlyCollection<UAV> availableUavs
    )
    {
        AssignmentFitnessBreakdown breakdown = BuildFitnessBreakdown(assignments);
        List<AssignmentPairingInsight> pairingInsights = assignments
            .Select(assignment => BuildPairingInsight(assignment, availableUavs))
            .ToList();

        return new AssignmentExplainability
        {
            FitnessBreakdown = breakdown,
            PairingInsights = pairingInsights,
        };
    }

    private AssignmentFitnessBreakdown BuildFitnessBreakdown(List<AssignmentGene> assignments)
    {
        if (assignments.Count == 0)
        {
            return new AssignmentFitnessBreakdown();
        }

        double telemetryScore = CalculateTelemetryScore(assignments);
        double distanceScore = CalculateDistanceScore(assignments);
        double priorityScore = CalculatePriorityScore(assignments);
        double timeOverlapPenalty = CalculateOverlapPenalty(assignments);
        double typeMismatchPenalty = CalculateMismatchPenalty(assignments);
        double missionCoverageBonus = CalculateMissionCoverageBonus(assignments);
        double activeMissionPenalty = CalculateActiveMissionPenalty(assignments);
        double totalFitnessScore =
            telemetryScore
            + distanceScore
            + priorityScore
            + timeOverlapPenalty
            + typeMismatchPenalty
            + missionCoverageBonus
            + activeMissionPenalty;

        return new AssignmentFitnessBreakdown
        {
            TelemetryScore = telemetryScore,
            DistanceScore = distanceScore,
            PriorityScore = priorityScore,
            TimeOverlapPenalty = timeOverlapPenalty,
            TypeMismatchPenalty = typeMismatchPenalty,
            MissionCoverageBonus = missionCoverageBonus,
            ActiveMissionPenalty = activeMissionPenalty,
            TotalFitnessScore = totalFitnessScore,
        };
    }

    private AssignmentPairingInsight BuildPairingInsight(
        AssignmentGene assignment,
        IReadOnlyCollection<UAV> availableUavs
    )
    {
        AssignmentPairingAlternative selectedScore = BuildPairingAlternative(assignment, assignment.UAV);
        List<AssignmentPairingAlternative> alternatives = availableUavs
            .Where(uav => uav.TailId != assignment.UAV.TailId)
            .Select(uav => BuildPairingAlternative(assignment, uav))
            .OrderByDescending(alt => alt.TotalScore)
            .Take(MissionServiceConstants.Explainability.MAX_ALTERNATIVES_PER_MISSION)
            .ToList();

        return new AssignmentPairingInsight
        {
            MissionId = assignment.Mission.Id,
            SuggestedTailId = assignment.UAV.TailId,
            TelemetryScore = selectedScore.TelemetryScore,
            DistanceScore = selectedScore.DistanceScore,
            PriorityScore = selectedScore.PriorityScore,
            TypeMismatchPenalty = selectedScore.TypeMismatchPenalty,
            ActiveMissionPenalty = selectedScore.ActiveMissionPenalty,
            TotalScore = selectedScore.TotalScore,
            Alternatives = alternatives,
        };
    }

    private AssignmentPairingAlternative BuildPairingAlternative(AssignmentGene referenceAssignment, UAV candidate)
    {
        Dictionary<TelemetryFields, double> telemetryWeights = _telemetryWeights.GetWeights(
            referenceAssignment.Mission.RequiredUAVType
        );
        double rawTelemetryScore = TelemetryScoreCalculator.CalculateAssignmentTelemetryScore(
            telemetryWeights,
            candidate.TelemetryData,
            referenceAssignment.Mission.Location
        );
        Location uavLocation = LocationProximityCalculator.ExtractUAVLocation(candidate.TelemetryData);
        double proximityScore = LocationProximityCalculator.CalculateNormelizedProximityScore(
            uavLocation,
            referenceAssignment.Mission.Location
        );
        double telemetryScore = rawTelemetryScore * _fitnessWeights.TelemetryOptimization;
        double distanceScore = proximityScore * _fitnessWeights.DistanceWeight;
        double priorityScore = (int)referenceAssignment.Mission.Priority * _fitnessWeights.PriorityCoverage;
        double typeMismatchPenalty =
            referenceAssignment.Mission.RequiredUAVType != candidate.UavType
                ? _fitnessWeights.TypeMismatchPenalty
                : 0;
        double activeMissionPenalty =
            _uavStatusService.GetActiveMission(candidate.TailId) != null
                ? _fitnessWeights.ActiveMissionPenalty
                : 0;
        double totalScore =
            telemetryScore
            + distanceScore
            + priorityScore
            + typeMismatchPenalty
            + activeMissionPenalty;

        return new AssignmentPairingAlternative
        {
            TailId = candidate.TailId,
            TelemetryScore = telemetryScore,
            DistanceScore = distanceScore,
            PriorityScore = priorityScore,
            TypeMismatchPenalty = typeMismatchPenalty,
            ActiveMissionPenalty = activeMissionPenalty,
            TotalScore = totalScore,
        };
    }

    private double CalculateTelemetryScore(List<AssignmentGene> assignments)
    {
        if (assignments.Count == 0)
        {
            return 0.0;
        }

        double totalWeightedTelemetryScore = 0.0;
        foreach (AssignmentGene assignment in assignments)
        {
            Dictionary<TelemetryFields, double> telemetryWeightsForUAVType =
                _telemetryWeights.GetWeights(assignment.Mission.RequiredUAVType);
            Dictionary<TelemetryFields, double> uavTelemetryData = assignment.UAV.TelemetryData;
            double assignmentTelemetryScore =
                TelemetryScoreCalculator.CalculateAssignmentTelemetryScore(
                    telemetryWeightsForUAVType,
                    uavTelemetryData,
                    assignment.Mission.Location
                );
            totalWeightedTelemetryScore += assignmentTelemetryScore;
        }
        double averageTelemetryScore = totalWeightedTelemetryScore / assignments.Count;
        return averageTelemetryScore * _fitnessWeights.TelemetryOptimization;
    }

    private double CalculateDistanceScore(List<AssignmentGene> assignments)
    {
        if (assignments.Count == 0)
        {
            return 0.0;
        }

        double totalProximityScore = 0.0;
        foreach (AssignmentGene assignment in assignments)
        {
            Location uavLocation = LocationProximityCalculator.ExtractUAVLocation(assignment.UAV.TelemetryData);
            double proximityScore = LocationProximityCalculator.CalculateNormelizedProximityScore(
                uavLocation,
                assignment.Mission.Location
            );
            totalProximityScore += proximityScore;
        }
        double averageProximityScore = totalProximityScore / assignments.Count;
        return averageProximityScore * _fitnessWeights.DistanceWeight;
    }

    private double CalculatePriorityScore(List<AssignmentGene> assignments)
    {
        double totalPriority = MissionAnalyzer.CalculateTotalPriority(assignments);
        return totalPriority * _fitnessWeights.PriorityCoverage;
    }

    private double CalculateMissionCoverageBonus(List<AssignmentGene> assignments)
    {
        HashSet<string> uniqueMissionIds = MissionAnalyzer.GetUniqueMissionIds(assignments);
        int uniqueCount = uniqueMissionIds.Count;
        return uniqueCount * uniqueCount * _fitnessWeights.MissionCoverageWeight;
    }

    private double CalculateOverlapPenalty(List<AssignmentGene> assignments)
    {
        if (assignments.Count <= 1)
        {
            return 0.0;
        }

        Dictionary<int, List<AssignmentGene>> assignmentsByUAVTailId =
            TimeOverlapDetector.GroupAssignmentsByUAVTailId(assignments);
        int totalOverlapCount = TimeOverlapDetector.CountTimeOverlaps(assignmentsByUAVTailId);
        return totalOverlapCount * _fitnessWeights.TimeOverlapPenalty;
    }

    private double CalculateMismatchPenalty(List<AssignmentGene> assignments)
    {
        int mismatchCount = MissionAnalyzer.CountTypeMismatches(assignments);
        return mismatchCount * _fitnessWeights.TypeMismatchPenalty;
    }

    private double CalculateActiveMissionPenalty(List<AssignmentGene> assignments)
    {
        int activeMissionCount = assignments.Count(a => _uavStatusService.GetActiveMission(a.UAV.TailId) != null);
        return activeMissionCount * _fitnessWeights.ActiveMissionPenalty;
    }
}
