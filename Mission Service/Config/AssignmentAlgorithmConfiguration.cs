// Services/Genetic_Assignment_Algorithm/IPopulationInitializer.cs
using Mission_Service.Models;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm
{
    public interface IPopulationInitializer
    {
        List<AssignmentChromosome> CreateInitialPopulation(
            List<Mission> missions,
            List<UAV> uavs);
    }
}