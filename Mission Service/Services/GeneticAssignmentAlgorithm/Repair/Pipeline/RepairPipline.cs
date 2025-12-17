using Mission_Service.Common.Constants;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Pipeline.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Strategies.Interfaces;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Pipeline
{
    public class RepairPipline : IRepairPipeline
    {
        private readonly IEnumerable<IRepairStrategy> _repairStrategies;

        public RepairPipline(IEnumerable<IRepairStrategy> repairStrategies)
        {
            _repairStrategies = repairStrategies;
        }

        public void RepairChromosomeViolaitions(
            AssignmentChromosome assignmentChromosome,
            IEnumerable<Mission> missions,
            IEnumerable<UAV> uavs
        )
        {
            for (
                int attempts = 0;
                attempts < MissionServiceConstants.MainAlgorithm.MAX_REPAIR_ATTEMPTS;
                attempts++
            )
            {
                int assignmentCountBeforeRepair = assignmentChromosome.AssignmentCount;

                foreach (IRepairStrategy repairStrategy in _repairStrategies)
                {
                    repairStrategy.RepairChromosomeViolation(assignmentChromosome, missions, uavs);
                }

                int assignmentCountAfterRepair = assignmentChromosome.AssignmentCount;

                if (
                    assignmentCountBeforeRepair == assignmentCountAfterRepair
                    && assignmentChromosome.IsValid
                )
                {
                    break;
                }
            }

            HashSet<string> assignedMissionIds = assignmentChromosome.AssignmentsList
                .Select(a => a.Mission.Id)
                .ToHashSet();

            assignmentChromosome.IsValid =
                missions.All(m => assignedMissionIds.Contains(m.Id)) && assignmentChromosome.IsValid;
        }
    }
}
