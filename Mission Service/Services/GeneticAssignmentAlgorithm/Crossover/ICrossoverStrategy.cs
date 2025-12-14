using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Crossover
{
    public interface ICrossoverStrategy
    {
        CrossoverResult CrossoverChromosomes(
            AssignmentChromosome firstChromosome,
            AssignmentChromosome secondChromosome
        );
    }
}
