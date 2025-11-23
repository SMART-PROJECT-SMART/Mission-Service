using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Repair
{
    public class TimeWindowRepairStrategy : IRepairStrategy
    {
        public void RepairChromosomeViolation(AssignmentChromosome assignmentChromosome, List<Mission> missions,
            List<UAV> uavs)
        {
            for (int assignmentIndex = assignmentChromosome.Assignments.Count() - 1;
                 assignmentIndex >= 0;
                 assignmentIndex--)
            {
                AssignmentGene currentGene = assignmentChromosome.Assignments.ElementAt(assignmentIndex);

                if (IsAssignmentTimeWindowValid(currentGene))
                    continue;

                DateTime validStartTime = CalculateValidStartTime(currentGene);
                currentGene.StartTime = validStartTime;

                TimeSpan validDuration = CalculateValidDuration(currentGene);

                if (validDuration >= currentGene.Mission.MinAssignmentLength)
                {
                    currentGene.Duration = validDuration;
                }
                else
                {
                    assignmentChromosome.Assignments.RemoveAt(assignmentIndex);
                }
            }
        }

        private bool IsAssignmentTimeWindowValid(AssignmentGene gene)
        {
            DateTime missionStartTime = gene.Mission.EarliestStartTime;
            DateTime missionEndTime = gene.Mission.LatestEndTime;
            DateTime uavAvailableFrom = gene.UAV.AvailableFrom;
            DateTime uavAvailableTo = gene.UAV.AvailableTo;

            bool assignmentStartValid = gene.StartTime >= missionStartTime && gene.StartTime >= uavAvailableFrom;
            bool assignmentEndValid = gene.EndTime <= missionEndTime && gene.EndTime <= uavAvailableTo;

            return assignmentStartValid && assignmentEndValid;
        }

        private DateTime CalculateValidStartTime(AssignmentGene gene)
        {
            DateTime missionEarliestStart = gene.Mission.EarliestStartTime;
            DateTime uavAvailableFrom = gene.UAV.AvailableFrom;

            DateTime validStartTime = missionEarliestStart > uavAvailableFrom
                ? missionEarliestStart
                : uavAvailableFrom;

            return validStartTime;
        }

        private TimeSpan CalculateValidDuration(AssignmentGene gene)
        {
            DateTime missionLatestEnd = gene.Mission.LatestEndTime;
            DateTime uavAvailableTo = gene.UAV.AvailableTo;

            DateTime effectiveEndTime = missionLatestEnd < uavAvailableTo
                ? missionLatestEnd
                : uavAvailableTo;

            TimeSpan maxPossibleDuration = effectiveEndTime - gene.StartTime;

            TimeSpan validDuration = TimeSpan.FromMinutes(
                Math.Min(gene.Mission.MaxAssignmentLength.TotalMinutes, maxPossibleDuration.TotalMinutes)
            );

            return validDuration;
        }
    }
}