using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Population.PopulationInitilizer
{
    public interface IPopulationInitializer
    {
        IEnumerable<AssignmentChromosome> CreateInitialPopulation(
            IEnumerable<Mission> missions,
            IEnumerable<UAV> uavs
        );
    }
}
