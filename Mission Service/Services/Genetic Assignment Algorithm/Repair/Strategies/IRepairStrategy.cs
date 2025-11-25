using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Repair
{
    public interface IRepairStrategy
    {
        void RepairChromosomeViolation(
            AssignmentChromosome assignmentChromosome,
            List<Mission> missions,
            List<UAV> uavs
        );
    }
}
