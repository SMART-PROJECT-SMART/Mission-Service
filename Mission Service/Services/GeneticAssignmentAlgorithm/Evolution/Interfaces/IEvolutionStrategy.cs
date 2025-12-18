using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Evolution.Interfaces
{
    public interface IEvolutionStrategy
    {
        IReadOnlyList<AssignmentChromosome> CreateNextGeneration(
            IReadOnlyList<AssignmentChromosome> currentPopulation,
            IEnumerable<Mission> missions,
            IEnumerable<UAV> availableUAVs
        );
    }
}
