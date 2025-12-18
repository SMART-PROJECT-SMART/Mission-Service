using Mission_Service.Extensions;
using Mission_Service.Models.choromosomes;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Selection.Elite.Interfaces;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Selection.Elite;

public class EliteSelector : IEliteSelector
{
    public IEnumerable<AssignmentChromosome> SelectElite(
        IEnumerable<AssignmentChromosome> chromosomePopulation,
        double elitePercentageOfPopulation,
        int totalPopulationSize
    )
    {
        int numberOfEliteChromosomesToSelect = CalculateEliteChromosomeCount(
            totalPopulationSize,
            elitePercentageOfPopulation
        );

        if (numberOfEliteChromosomesToSelect == 0)
        {
            return Enumerable.Empty<AssignmentChromosome>();
        }

        if (numberOfEliteChromosomesToSelect == 1)
        {
            AssignmentChromosome singleBestChromosome = chromosomePopulation.MaxBy(c =>
                c.FitnessScore
            )!;
            return [singleBestChromosome];
        }

        return SelectTopChromosomesByFitnessScore(
            chromosomePopulation,
            numberOfEliteChromosomesToSelect
        );
    }

    private int CalculateEliteChromosomeCount(int totalPopulationSize, double elitePercentage)
    {
        return (int)(totalPopulationSize * elitePercentage);
    }

    private IEnumerable<AssignmentChromosome> SelectTopChromosomesByFitnessScore(
        IEnumerable<AssignmentChromosome> chromosomePopulation,
        int numberOfChromosomesToSelect
    )
    {
        return chromosomePopulation
            .OrderByDescending(chromosome => chromosome.FitnessScore)
            .Take(numberOfChromosomesToSelect);
    }
}
