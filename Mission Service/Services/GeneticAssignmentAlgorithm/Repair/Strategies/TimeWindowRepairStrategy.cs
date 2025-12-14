using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Strategies
{
    public class TimeWindowRepairStrategy : IRepairStrategy
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

            IEnumerable<AssignmentGene> repairedAssignments = assignmentChromosome
                .Assignments.Select(gene => RepairGeneIfNeeded(gene))
                .Where(gene => gene != null)
                .Cast<AssignmentGene>();

            assignmentChromosome.Assignments = repairedAssignments;
        }

        private AssignmentGene RepairGeneIfNeeded(AssignmentGene gene)
        {
            if (IsAssignmentTimeWindowValid(gene))
            {
                return gene;
            }

            DateTime validStartTime = CalculateValidStartTime(gene);
            gene.StartTime = validStartTime;

            TimeSpan validDuration = CalculateValidDuration(gene);

            if (validDuration <= TimeSpan.Zero)
            {
                return null;
            }

            gene.Duration = validDuration;
            return gene;
        }

        private bool IsAssignmentTimeWindowValid(AssignmentGene gene)
        {
            DateTime missionStart = gene.Mission.TimeWindow.Start;
            DateTime missionEnd = gene.Mission.TimeWindow.End;

            bool assignmentStartValid = gene.StartTime >= missionStart;
            bool assignmentEndValid = gene.EndTime <= missionEnd;

            return assignmentStartValid && assignmentEndValid;
        }

        private DateTime CalculateValidStartTime(AssignmentGene gene)
        {
            DateTime missionStart = gene.Mission.TimeWindow.Start;
            return gene.StartTime < missionStart ? missionStart : gene.StartTime;
        }

        private TimeSpan CalculateValidDuration(AssignmentGene gene)
        {
            DateTime missionEnd = gene.Mission.TimeWindow.End;
            TimeSpan maxPossibleDuration = missionEnd - gene.StartTime;
            return maxPossibleDuration;
        }
    }
}
