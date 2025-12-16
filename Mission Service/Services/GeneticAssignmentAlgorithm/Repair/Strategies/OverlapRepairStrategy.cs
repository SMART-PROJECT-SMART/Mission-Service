using Mission_Service.Models;
using Mission_Service.Models.choromosomes;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Strategies.Interfaces;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Strategies
{
    public class OverlapRepairStrategy : IRepairStrategy
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
            IEnumerable<IGrouping<int, AssignmentGene>> groupedByUAV = assignmentList.GroupBy(a =>
                a.UAV.TailId
            );

            HashSet<AssignmentGene> assignmentsToRemove = new HashSet<AssignmentGene>();

            foreach (IGrouping<int, AssignmentGene> uavGroup in groupedByUAV)
            {
                CollectOverlappingAssignments(uavGroup, assignmentsToRemove);
            }

            assignmentChromosome.Assignments = assignmentList.Except(assignmentsToRemove);
        }

        private void CollectOverlappingAssignments(
            IEnumerable<AssignmentGene> assignments,
            HashSet<AssignmentGene> assignmentsToRemove
        )
        {
            AssignmentGene[] sortedAssignments = assignments
                .OrderBy(a => a.StartTime)
                .ToArray();

            if (sortedAssignments.Length < 2)
            {
                return;
            }

            for (int i = 0; i < sortedAssignments.Length - 1; i++)
            {
                AssignmentGene currentAssignment = sortedAssignments[i];

                if (assignmentsToRemove.Contains(currentAssignment))
                {
                    continue;
                }

                AssignmentGene nextAssignment = sortedAssignments[i + 1];

                if (HasOverlap(currentAssignment, nextAssignment))
                {
                    AssignmentGene assignmentToRemove = SelectAssignmentToRemove(
                        currentAssignment,
                        nextAssignment
                    );
                    assignmentsToRemove.Add(assignmentToRemove);
                }
            }
        }

        private bool HasOverlap(AssignmentGene current, AssignmentGene next)
        {
            return current.EndTime > next.StartTime;
        }

        private AssignmentGene SelectAssignmentToRemove(
            AssignmentGene assignment1,
            AssignmentGene assignment2
        )
        {
            int priority1 = (int)assignment1.Mission.Priority;
            int priority2 = (int)assignment2.Mission.Priority;

            if (priority1 > priority2)
            {
                return assignment2;
            }

            if (priority2 > priority1)
            {
                return assignment1;
            }

            return Random.Shared.Next(2) == 0 ? assignment1 : assignment2;
        }
    }
}
