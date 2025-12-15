using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Pipeline
{
    public interface IRepairPipeline
    {
        public void RepairChromosomeViolaitions(
            AssignmentChromosome assignmentChromosome,
            IEnumerable<Mission> missions,
            IEnumerable<UAV> uavs
        );
    }
}
