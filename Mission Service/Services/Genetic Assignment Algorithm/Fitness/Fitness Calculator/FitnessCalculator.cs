using Core.Common.Enums;
using Microsoft.Extensions.Options;
using Mission_Service.Config;
using Mission_Service.Extensions;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Fitness_Calculator;

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
            Dictionary<TelemetryFields, double> telemetryWeightsForUAVType = _telemetryWeights.GetWeights(
                assignment.Mission.RequiredUAVType
            );
            Dictionary<TelemetryFields, double> uavTelemetryData = assignment.UAV.TelemetryData;

            foreach (KeyValuePair<TelemetryFields, double> telemetryWeight in telemetryWeightsForUAVType)
            {
                totalWeightedTelemetryScore += telemetryWeight.CalculateWeightedTelemetryScore(uavTelemetryData);
            }
        }

        double averageTelemetryScore = totalWeightedTelemetryScore / assignments.Count;
        return averageTelemetryScore * _fitnessWeights.TelemetryOptimization;
    }

    private double CalculatePriorityScore(List<AssignmentGene> assignments)
    {
        HashSet<string> seenMissionIds = new HashSet<string>(assignments.Count);
        double totalPriority = 0.0;

        foreach (AssignmentGene assignment in assignments)
        {
            if (seenMissionIds.Add(assignment.Mission.Id))
            {
                totalPriority += (int)assignment.Mission.Priority;
            }
        }

        return totalPriority * _fitnessWeights.PriorityCoverage;
    }

    private double CalculateMissionCoverageBonus(List<AssignmentGene> assignments)
    {
        HashSet<string> uniqueMissionIds = new HashSet<string>(assignments.Count);

        foreach (AssignmentGene assignment in assignments)
        {
            uniqueMissionIds.Add(assignment.Mission.Id);
        }

        int uniqueCount = uniqueMissionIds.Count;
        return uniqueCount * uniqueCount * _fitnessWeights.MissionCoverageWeight;
    }

    private double CalculateOverlapPenalty(List<AssignmentGene> assignments)
    {
        if (assignments.Count <= 1)
            return 0.0;

        Dictionary<int, List<AssignmentGene>> assignmentsByUAVTailId = GroupAssignmentsByUAVTailId(assignments);
        int totalOverlapCount = CountTimeOverlaps(assignmentsByUAVTailId);

        return totalOverlapCount * _fitnessWeights.TimeOverlapPenalty;
    }

    private Dictionary<int, List<AssignmentGene>> GroupAssignmentsByUAVTailId(List<AssignmentGene> assignments)
    {
        Dictionary<int, List<AssignmentGene>> uavGroups = new Dictionary<int, List<AssignmentGene>>();

        foreach (AssignmentGene assignment in assignments)
        {
            if (!uavGroups.TryGetValue(assignment.UAV.TailId, out List<AssignmentGene>? assignmentList))
            {
                assignmentList = new List<AssignmentGene>();
                uavGroups[assignment.UAV.TailId] = assignmentList;
            }
            assignmentList.Add(assignment);
        }

        return uavGroups;
    }

    private int CountTimeOverlaps(Dictionary<int, List<AssignmentGene>> assignmentsByUAVTailId)
    {
        int overlapCount = 0;

        foreach (List<AssignmentGene> uavAssignments in assignmentsByUAVTailId.Values)
        {
            if (uavAssignments.Count <= 1)
                continue;

            uavAssignments.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));

            for (int i = 0; i < uavAssignments.Count - 1; i++)
            {
                if (uavAssignments[i].EndTime > uavAssignments[i + 1].StartTime)
                {
                    overlapCount++;
                }
            }
        }

        return overlapCount;
    }

    private double CalculateMismatchPenalty(List<AssignmentGene> assignments)
    {
        int mismatchCount = 0;

        foreach (AssignmentGene assignment in assignments)
        {
            if (assignment.Mission.RequiredUAVType != assignment.UAV.UavType)
            {
                mismatchCount++;
            }
        }

        return mismatchCount * _fitnessWeights.TypeMismatchPenalty;
    }
}
