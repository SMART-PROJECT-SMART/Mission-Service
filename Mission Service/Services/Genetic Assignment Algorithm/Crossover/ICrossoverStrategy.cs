using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Crossover
{
    public interface ICrossoverStrategy
    {
        CrossoverResult CrossoverChromosomes(AssignmentChromosome firstChromosome,
            AssignmentChromosome secondChromosome);
    }
}
