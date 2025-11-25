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
        double total = 0.0;

        foreach (AssignmentGene assignment in chromosome.Assignments)
        {
            Dictionary<TelemetryFields, double> assignmentWeights = _telemetryWeights.GetWeights(
                assignment.Mission.RequiredUAVType
            );

            foreach (KeyValuePair<TelemetryFields, double> assignmentPair in assignmentWeights)
            {
                if (assignment.UAV.TelemetryData.TryGetValue(assignmentPair.Key, out double value))
                {
                    double normalized = assignmentPair.Key.NormalizeTelemetryValue(value);
                    total += normalized * assignmentPair.Value;
                }
            }
        }

        int count = chromosome.Assignments.Count();
        double average = count > 0 ? total / count : 0.0;

        return average * _fitnessWeights.TelemetryOptimization;
    }

    private double CalculatePriorityScore(AssignmentChromosome chromosome)
    {
        // Only count priority for unique missions (no duplicates)
        double totalPriority = chromosome.Assignments
            .GroupBy(a => a.Mission.Id)
            .Select(g => g.First())
            .Sum(a => (int)a.Mission.Priority);

        return totalPriority * _fitnessWeights.PriorityCoverage;
    }

    private double CalculateMissionCoverageBonus(AssignmentChromosome chromosome)
    {
        // Give bonus for covering more unique missions
        int uniqueMissions = chromosome.Assignments
            .Select(a => a.Mission.Id)
         .Distinct()
            .Count();

        // Much stronger bonus - mission coverage is critical!
        // Use exponential scaling to heavily reward covering more missions
     return uniqueMissions * uniqueMissions * 1000.0;
    }

    private double CalculateOverlapPenalty(AssignmentChromosome chromosome)
    {
        int overlapCount = 0;

        IEnumerable<IGrouping<int, AssignmentGene>> assignmentsByUav =
            chromosome.Assignments.GroupBy(a => a.UAV.TailId);

        foreach (IGrouping<int, AssignmentGene> uavAssignments in assignmentsByUav)
        {
            List<AssignmentGene> chronologicalAssignments = uavAssignments
                .OrderBy(a => a.StartTime)
                .ToList();

            for (int i = 0; i < chronologicalAssignments.Count - 1; i++)
            {
                DateTime currentAssignmentEnd = chronologicalAssignments[i]
                    .StartTime.Add(chronologicalAssignments[i].Duration);
                if (currentAssignmentEnd > chronologicalAssignments[i + 1].StartTime)
                {
                    overlapCount++;
                }
            }
        }

        return overlapCount * _fitnessWeights.TimeOverlapPenalty;
    }

    private double CalculateMismatchPenalty(AssignmentChromosome chromosome)
    {
        int mismatchCount = chromosome.Assignments.Count(a =>
            a.Mission.RequiredUAVType != a.UAV.UavType
        );

        return mismatchCount * _fitnessWeights.TypeMismatchPenalty;
    }
}
