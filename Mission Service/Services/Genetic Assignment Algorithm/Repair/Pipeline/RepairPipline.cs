using Mission_Service.Common.Constants;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Repair.Pipeline
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
            List<Mission> missions,
            List<UAV> uavs
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

            List<AssignmentGene> finalAssignments = assignmentChromosome.AssignmentsList;
            HashSet<string> assignedMissionIds = new HashSet<string>(finalAssignments.Count);

            foreach (AssignmentGene assignment in finalAssignments)
            {
                assignedMissionIds.Add(assignment.Mission.Id);
            }

            bool allMissionsAssigned = missions.All(m => assignedMissionIds.Contains(m.Id));
            assignmentChromosome.IsValid = allMissionsAssigned && assignmentChromosome.IsValid;
        }
    }
}
