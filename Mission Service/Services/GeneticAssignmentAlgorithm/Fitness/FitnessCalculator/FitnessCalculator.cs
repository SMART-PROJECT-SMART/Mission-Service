using Core.Common.Enums;
using Microsoft.Extensions.Options;
using Mission_Service.Config;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Fitness.FitnessCalculator.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Fitness.Helpers;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Fitness.FitnessCalculator;

public class FitnessCalculator : IFitnessCalculator
{
    private readonly TelemetryWeightsConfiguration _telemetryWeights;
    private readonly FitnessWeightsConfiguration _fitnessWeights;

    public FitnessCalculator(
        IOptions<TelemetryWeightsConfiguration> telemetryWeights,
        IOptions<FitnessWeightsConfiguration> fitnessWeights
    )
    {
        _telemetryWeights = telemetryWeights.Value;
        _fitnessWeights = fitnessWeights.Value;
    }

    public double CalculateFitness(AssignmentChromosome chromosome)
    {
        List<AssignmentGene> assignments = chromosome.AssignmentsList;

        double totalFitness = 0.0;
        totalFitness += CalculateTelemetryScore(assignments);
        totalFitness += CalculatePriorityScore(assignments);
        totalFitness += CalculateOverlapPenalty(assignments);
        totalFitness += CalculateMismatchPenalty(assignments);
        totalFitness += CalculateMissionCoverageBonus(assignments);
        totalFitness += CalculateActiveMissionPenalty(assignments);

        chromosome.FitnessScore = totalFitness;
        return totalFitness;
    }

    private double CalculateTelemetryScore(List<AssignmentGene> assignments)
    {
        if (assignments.Count == 0)
            return 0.0;

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
            return 0.0;

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
        int activeMissionCount = assignments.Count(a => a.UAV.ActiveMission != null);
        return activeMissionCount * _fitnessWeights.ActiveMissionPenalty;
    }
}
