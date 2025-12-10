using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Selection.Elite;

public class EliteSelector : IEliteSelector
{
    public List<AssignmentChromosome> SelectElite(
        List<AssignmentChromosome> population,
        double elitePercentage,
        int populationSize
    )
    {
        int numberOfEliteChromosomes = CalculateEliteCount(populationSize, elitePercentage);

        if (numberOfEliteChromosomes == 0)
        {
            return new List<AssignmentChromosome>();
        }

        if (numberOfEliteChromosomes == 1)
        {
            AssignmentChromosome bestChromosome = FindChromosomeWithHighestFitness(population);
            return new List<AssignmentChromosome> { bestChromosome };
        }

        return SelectTopChromosomesByFitness(population, numberOfEliteChromosomes);
    }

    private int CalculateEliteCount(int populationSize, double elitePercentage)
    {
        return (int)(populationSize * elitePercentage);
    }

    private List<AssignmentChromosome> SelectTopChromosomesByFitness(
        List<AssignmentChromosome> population,
        int count
    )
    {
        return population
            .OrderByDescending(chromosome => chromosome.FitnessScore)
            .Take(count)
            .ToList();
    }

    private AssignmentChromosome FindChromosomeWithHighestFitness(
        List<AssignmentChromosome> population
    )
    {
        AssignmentChromosome chromosomeWithBestFitness = population[0];
        double highestFitnessScore = chromosomeWithBestFitness.FitnessScore;

        for (int i = 1; i < population.Count; i++)
        {
            AssignmentChromosome currentChromosome = population[i];
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
