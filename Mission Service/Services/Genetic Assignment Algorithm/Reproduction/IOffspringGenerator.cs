using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Reproduction;

public interface IOffspringGenerator
{
    List<AssignmentChromosome> CreateOffspring(
        List<AssignmentChromosome> population,
        int offspringCount,
        List<UAV> uavs
    );
}
