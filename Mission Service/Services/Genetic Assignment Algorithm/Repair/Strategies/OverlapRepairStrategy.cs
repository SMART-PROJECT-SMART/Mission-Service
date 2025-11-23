using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Repair.Strategies
{
    public class OverlapRepairStrategy : IRepairStrategy
    {
        public void RepairChromosomeViolation(
            AssignmentChromosome assignmentChromosome,
            List<Mission> missions,
            List<UAV> uavs)
        {
            IEnumerable<IGrouping<int, AssignmentGene>> groupedByUAV =
                assignmentChromosome.Assignments.GroupBy(a => a.UAV.TailId);

            foreach (IGrouping<int, AssignmentGene> uavGroup in groupedByUAV)
            {
                RepairOverlapsForUAV(uavGroup.ToList(), assignmentChromosome);
            }
        }

        private void RepairOverlapsForUAV(List<AssignmentGene> assignments, AssignmentChromosome chromosome)
        {
            List<AssignmentGene> sortedAssignments = assignments.OrderBy(a => a.StartTime).ToList();

            for (int i = 0; i < sortedAssignments.Count - 1; i++)
            {
                AssignmentGene currentAssignment = sortedAssignments[i];
                AssignmentGene nextAssignment = sortedAssignments[i + 1];

                if (HasOverlap(currentAssignment, nextAssignment))
                {
                    AssignmentGene assignmentToRemove = SelectAssignmentToRemove(currentAssignment, nextAssignment);
                    chromosome.Assignments.Remove(assignmentToRemove);
                    break;
                }
            }
        }

        private bool HasOverlap(AssignmentGene current, AssignmentGene next)
        {
            return current.EndTime > next.StartTime;
        }

        private AssignmentGene SelectAssignmentToRemove(AssignmentGene assignment1, AssignmentGene assignment2)
        {
            if (assignment1.Mission.Priority > assignment2.Mission.Priority)
            {
                return assignment2;
            }
            else if (assignment2.Mission.Priority > assignment1.Mission.Priority)
            {
                return assignment1;
            }
            else
            {
                return Random.Shared.Next(2) == 0 ? assignment1 : assignment2;
            }
        }
    }
}