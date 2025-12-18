using Mission_Service.Models;
using Mission_Service.Models.choromosomes;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Strategies.Interfaces;

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
            if (IsChromosomeEmpty(assignmentChromosome))
            {
                return;
            }

            IEnumerable<AssignmentGene> repairedAssignments = RepairAllGenes(
                assignmentChromosome.Assignments
            );
            assignmentChromosome.Assignments = repairedAssignments;
        }

        private bool IsChromosomeEmpty(AssignmentChromosome assignmentChromosome)
        {
            return assignmentChromosome?.Assignments == null
                || !assignmentChromosome.Assignments.Any();
        }

        private IEnumerable<AssignmentGene> RepairAllGenes(IEnumerable<AssignmentGene> assignments)
        {
            IEnumerable<AssignmentGene?> repairedGenes = assignments.Select(TryRepairGene
            );
            IEnumerable<AssignmentGene?> validGenes = repairedGenes.Where(gene => gene != null);
            return validGenes.Cast<AssignmentGene>();
        }

        private AssignmentGene? TryRepairGene(AssignmentGene assignmentGene)
        {
            return IsGeneTimeWindowValid(assignmentGene) ? assignmentGene : AttemptToShiftGeneIntoTimeWindow(assignmentGene);
        }

        private bool IsGeneTimeWindowValid(AssignmentGene assignmentGene)
        {
            DateTime missionWindowStart = assignmentGene.Mission.TimeWindow.Start;
            DateTime missionWindowEnd = assignmentGene.Mission.TimeWindow.End;

            bool startsWithinWindow = assignmentGene.StartTime >= missionWindowStart;
            bool endsWithinWindow = assignmentGene.EndTime <= missionWindowEnd;

            return startsWithinWindow && endsWithinWindow;
        }

        private AssignmentGene? AttemptToShiftGeneIntoTimeWindow(AssignmentGene assignmentGene)
        {
            TimeSpan originalMissionDuration = assignmentGene.Duration;
            DateTime adjustedStartTime = CalculateAdjustedStartTime(assignmentGene);
            DateTime projectedEndTime = adjustedStartTime + originalMissionDuration;

            if (!DoesMissionFitInWindow(projectedEndTime, assignmentGene.Mission.TimeWindow.End)) return null;
            assignmentGene.StartTime = adjustedStartTime;
            return assignmentGene;

        }

        private DateTime CalculateAdjustedStartTime(AssignmentGene assignmentGene)
        {
            DateTime missionWindowStart = assignmentGene.Mission.TimeWindow.Start;
            DateTime currentStartTime = assignmentGene.StartTime;

            bool startsBeforeWindow = currentStartTime < missionWindowStart;
            return startsBeforeWindow ? missionWindowStart : currentStartTime;
        }

        private bool DoesMissionFitInWindow(DateTime projectedEndTime, DateTime missionWindowEnd)
        {
            return projectedEndTime <= missionWindowEnd;
        }
    }
}
