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
        double total = 0.0;

        total += CalculateTelemetryScore(chromosome);
        total += CalculatePriorityScore(chromosome);
        total += CalculateOverlapPenalty(chromosome);
        total += CalculateMismatchPenalty(chromosome);
        total += CalculateMissionCoverageBonus(chromosome);

        chromosome.FitnessScore = total;

        return total;
    }

    private double CalculateTelemetryScore(AssignmentChromosome chromosome)
    {
        List<AssignmentGene> assignments = chromosome.AssignmentsList;
        if (assignments.Count == 0)
            return 0.0;

        double total = 0.0;

        foreach (AssignmentGene assignment in assignments)
        {
            Dictionary<TelemetryFields, double> assignmentWeights = _telemetryWeights.GetWeights(assignment.Mission.RequiredUAVType);
            Dictionary<TelemetryFields, double> telemetryData = assignment.UAV.TelemetryData;

            foreach (KeyValuePair<TelemetryFields, double> weightEntry in assignmentWeights)
            {
                if (telemetryData.TryGetValue(weightEntry.Key, out double value))
                {
                    double normalized = weightEntry.Key.NormalizeTelemetryValue(value);
                    total += normalized * weightEntry.Value;
                }
            }
        }

        double average = total / assignments.Count;
        return average * _fitnessWeights.TelemetryOptimization;
    }

    private double CalculatePriorityScore(AssignmentChromosome chromosome)
    {
        List<AssignmentGene> assignments = chromosome.AssignmentsList;
        HashSet<string> seenMissions = new HashSet<string>(assignments.Count);
        double totalPriority = 0.0;

        foreach (AssignmentGene assignment in assignments)
        {
            if (seenMissions.Add(assignment.Mission.Id))
            {
                totalPriority += (int)assignment.Mission.Priority;
            }
        }

        return totalPriority * _fitnessWeights.PriorityCoverage;
    }

    private double CalculateMissionCoverageBonus(AssignmentChromosome chromosome)
    {
        List<AssignmentGene> assignments = chromosome.AssignmentsList;
        HashSet<string> uniqueMissions = new HashSet<string>(assignments.Count);

        foreach (AssignmentGene assignment in assignments)
        {
            uniqueMissions.Add(assignment.Mission.Id);
        }

        int count = uniqueMissions.Count;
        return count * count * _fitnessWeights.MissionCoverageWeight;
    }

    private double CalculateOverlapPenalty(AssignmentChromosome chromosome)
    {
        List<AssignmentGene> assignments = chromosome.AssignmentsList;
        if (assignments.Count <= 1)
            return 0.0;

        Dictionary<int, List<AssignmentGene>> uavGroups = new Dictionary<int, List<AssignmentGene>>();
        foreach (AssignmentGene assignment in assignments)
        {
            if (!uavGroups.TryGetValue(assignment.UAV.TailId, out List<AssignmentGene>? list))
            {
                list = new List<AssignmentGene>();
                uavGroups[assignment.UAV.TailId] = list;
            }
            list.Add(assignment);
        }

        int overlapCount = 0;

        foreach (List<AssignmentGene> uavAssignments in uavGroups.Values)
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

        return overlapCount * _fitnessWeights.TimeOverlapPenalty;
    }

    private double CalculateMismatchPenalty(AssignmentChromosome chromosome)
    {
        List<AssignmentGene> assignments = chromosome.AssignmentsList;
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
