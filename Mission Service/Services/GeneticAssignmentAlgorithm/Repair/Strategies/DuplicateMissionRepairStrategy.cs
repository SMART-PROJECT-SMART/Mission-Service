using Mission_Service.Models;
using Mission_Service.Models.choromosomes;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Strategies.Interfaces;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Strategies
{
    public class DuplicateMissionRepairStrategy : IRepairStrategy
    {
        public void RepairChromosomeViolation(
            AssignmentChromosome assignmentChromosome,
            IEnumerable<Mission> missions,
            IEnumerable<UAV> uavs
        )
        {
            if (
                assignmentChromosome?.Assignments == null
                || !assignmentChromosome.Assignments.Any()
            )
            {
                return;
            }

            List<AssignmentGene> assignmentList = assignmentChromosome.AssignmentsList;
            HashSet<string> seenMissions = new HashSet<string>();
            List<AssignmentGene> uniqueAssignments = new List<AssignmentGene>();

            foreach (AssignmentGene assignment in assignmentList)
            {
                if (!seenMissions.Contains(assignment.Mission.Id))
                {
                    seenMissions.Add(assignment.Mission.Id);
                    uniqueAssignments.Add(assignment);
                }
            }

            assignmentChromosome.Assignments = uniqueAssignments;
        }
    }
}
