using Mission_Service.Extensions;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Selection.Elite;

public class EliteSelector : IEliteSelector
{
    public List<AssignmentChromosome> SelectElite(
        List<AssignmentChromosome> chromosomePopulation,
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
            return new List<AssignmentChromosome>();
        }

        if (numberOfEliteChromosomesToSelect == 1)
        {
            AssignmentChromosome singleBestChromosome =
                chromosomePopulation.FindChromosomeWithHighestFitness();
            return new List<AssignmentChromosome> { singleBestChromosome };
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

    private List<AssignmentChromosome> SelectTopChromosomesByFitnessScore(
        List<AssignmentChromosome> chromosomePopulation,
        int numberOfChromosomesToSelect
    )
    {
        return chromosomePopulation
            .OrderByDescending(chromosome => chromosome.FitnessScore)
            .Take(numberOfChromosomesToSelect)
            .ToList();
    }
}
