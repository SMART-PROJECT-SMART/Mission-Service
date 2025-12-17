using Mission_Service.Models.choromosomes;

namespace Mission_Service.Extensions;

public static class ChromosomeCollectionExtensions
{
    public static IEnumerable<AssignmentChromosome> FilterValidChromosomes(
        this IEnumerable<AssignmentChromosome> chromosomes
    )
    {
        return chromosomes.Where(chromosome => chromosome.IsValid);
    }

    public static IEnumerable<AssignmentChromosome> FilterInvalidChromosomes(
        this IEnumerable<AssignmentChromosome> chromosomes
    )
    {
        return chromosomes.Where(chromosome => !chromosome.IsValid);
    }

    public static IEnumerable<AssignmentChromosome> OrderByBestQuality(
        this IEnumerable<AssignmentChromosome> chromosomes
    )
    {
        return chromosomes
            .OrderByDescending(chromosome => chromosome.AssignmentCount)
            .ThenByDescending(chromosome => chromosome.FitnessScore);
    }

    public static IEnumerable<AssignmentChromosome> FilterAndOrderInvalidChromosomesByQuality(
        this IEnumerable<AssignmentChromosome> chromosomes
    )
    {
        return chromosomes.FilterInvalidChromosomes().OrderByBestQuality();
    }

    public static AssignmentChromosome FindChromosomeWithHighestFitness(
        this IEnumerable<AssignmentChromosome> chromosomePopulation
    )
    {
        AssignmentChromosome best = chromosomePopulation.MaxBy(c => c.FitnessScore)!;

        return best;
    }
}
