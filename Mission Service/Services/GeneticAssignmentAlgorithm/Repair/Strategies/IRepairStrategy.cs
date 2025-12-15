using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Strategies
{
    public interface IRepairStrategy
    {
        void RepairChromosomeViolation(
            AssignmentChromosome assignmentChromosome,
            IEnumerable<Mission> missions,
            IEnumerable<UAV> uavs
        );
    }
}
