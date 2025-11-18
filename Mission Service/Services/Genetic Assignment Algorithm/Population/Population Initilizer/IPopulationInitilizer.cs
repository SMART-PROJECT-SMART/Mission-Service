using Mission_Service.Models;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Population.Population_Initilizer
{
    public interface IPopulationInitializer
    {
        IEnumerable<AssignmentChromosome> CreateInitialPopulation(
            IEnumerable<Mission> missions,
            IEnumerable<UAV> uavs);
    }
}