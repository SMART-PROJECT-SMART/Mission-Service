using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Reproduction;

public interface IOffspringGenerator
{
    IEnumerable<AssignmentChromosome> CreateOffspring(
        IReadOnlyList<AssignmentChromosome> parentChromosomePopulation,
        int numberOfOffspringChromosomesToGenerate,
        IReadOnlyList<UAV> availableUAVsForAssignment
    );
}
