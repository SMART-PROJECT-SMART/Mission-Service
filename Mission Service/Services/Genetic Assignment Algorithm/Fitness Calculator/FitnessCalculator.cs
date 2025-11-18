using Core.Common.Enums;
using Microsoft.Extensions.Options;
using Mission_Service.Config;
using Mission_Service.Extensions;
using Mission_Service.Models;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Fitness_Calculator;

public class FitnessCalculator : IFitnessCalculator
{
    private readonly TelemetryWeightsConfiguration _telemetryWeights;
    private readonly FitnessWeightsConfiguration _fitnessWeights;

    public FitnessCalculator(
        IOptions<TelemetryWeightsConfiguration> telemetryWeights,
        IOptions<FitnessWeightsConfiguration> fitnessWeights)
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

        chromosome.FitnessScore = total;
        chromosome.IsValid = total >= 0.0;

        return total;
    }

    private double CalculateTelemetryScore(AssignmentChromosome chromosome)
    {
        double total = 0.0;

        foreach (AssignmentGene assignment in chromosome.Assignments)
        {
            Dictionary<TelemetryFields, double> weights =
                _telemetryWeights.GetWeights(assignment.Mission.RequiredUAVType);

            foreach (KeyValuePair<TelemetryFields, double> entry in weights)
            {
                if (assignment.UAV.TelemetryData.TryGetValue(entry.Key, out double value))
                {
                    double normalized = entry.Key.NormalizeTelemetryValue(value);
                    total += normalized * entry.Value;
                }
            }
        }

        int count = chromosome.Assignments.Count();
        double average = count > 0 ? total / count : 0.0;

        return average * _fitnessWeights.TelemetryOptimization;
    }

    private double CalculatePriorityScore(AssignmentChromosome chromosome)
    {
        double totalPriority = chromosome.Assignments.Sum(a => (int)a.Mission.Priority);

        return totalPriority * _fitnessWeights.PriorityCoverage;
    }

    private double CalculateOverlapPenalty(AssignmentChromosome chromosome)
    {
        int overlapCount = 0;

        IEnumerable<IGrouping<int, AssignmentGene>> grouped =
            chromosome.Assignments.GroupBy(a => a.UAV.TailId);

        foreach (IGrouping<int, AssignmentGene> group in grouped)
        {
            List<AssignmentGene> sorted = group.OrderBy(a => a.StartTime).ToList();

            for (int i = 0; i < sorted.Count - 1; i++)
            {
                DateTime currentEnd = sorted[i].StartTime.Add(sorted[i].Duration);
                if (currentEnd > sorted[i + 1].StartTime)
                {
                    overlapCount++;
                }
            }
        }

        return overlapCount * _fitnessWeights.TimeOverlapPenalty;
    }

    private double CalculateMismatchPenalty(AssignmentChromosome chromosome)
    {
        int mismatchCount = chromosome.Assignments
            .Count(a => a.Mission.RequiredUAVType != a.UAV.UavType);

        return mismatchCount * _fitnessWeights.TypeMismatchPenalty;
    }
}