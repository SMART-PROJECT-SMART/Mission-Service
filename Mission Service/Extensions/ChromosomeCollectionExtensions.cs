using Mission_Service.Models.choromosomes;

namespace Mission_Service.Extensions;

public static class ChromosomeCollectionExtensions
{
    public static List<AssignmentChromosome> FilterValidChromosomes(
        this IEnumerable<AssignmentChromosome> chromosomes
    )
    {
        return chromosomes.Where(chromosome => chromosome.IsValid).ToList();
    }

    public static List<AssignmentChromosome> FilterInvalidChromosomes(
        this IEnumerable<AssignmentChromosome> chromosomes
    )
    {
        return chromosomes.Where(chromosome => !chromosome.IsValid).ToList();
    }

    public static List<AssignmentChromosome> OrderByBestQuality(
        this IEnumerable<AssignmentChromosome> chromosomes
    )
    {
        return chromosomes
            .OrderByDescending(chromosome => chromosome.AssignmentCount)
            .ThenByDescending(chromosome => chromosome.FitnessScore)
            .ToList();
    }

    public static List<AssignmentChromosome> FilterAndOrderInvalidChromosomesByQuality(
        this IEnumerable<AssignmentChromosome> chromosomes
    )
    {
        return chromosomes.FilterInvalidChromosomes().OrderByBestQuality();
    }

    public static AssignmentChromosome FindChromosomeWithHighestFitness(
        this List<AssignmentChromosome> chromosomePopulation
    )
    {
        if (chromosomePopulation == null || chromosomePopulation.Count == 0)
        {
            throw new ArgumentException(
                "Population cannot be null or empty",
                nameof(chromosomePopulation)
            );
        }

        AssignmentChromosome chromosomeWithBestFitness = chromosomePopulation[0];
        double highestFitnessScore = chromosomeWithBestFitness.FitnessScore;

        for (int i = 1; i < chromosomePopulation.Count; i++)
        {
            AssignmentChromosome currentChromosome = chromosomePopulation[i];
            double currentFitnessScore = currentChromosome.FitnessScore;

            if (currentFitnessScore > highestFitnessScore)
            {
                chromosomeWithBestFitness = currentChromosome;
                highestFitnessScore = currentFitnessScore;
            }
        }

        return chromosomeWithBestFitness;
    }
}
