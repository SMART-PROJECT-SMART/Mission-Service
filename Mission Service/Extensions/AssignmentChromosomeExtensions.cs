using Core.Common.Enums;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Extensions
{
    public static class AssignmentChromosomeExtensions
    {
        public static AssignmentResult ToAssignmentResult(
            this AssignmentChromosome chromosome,
            IEnumerable<UAV> allUAVs
        )
        {
            IEnumerable<MissionAssignmentPairing> pairings = chromosome.Assignments.Select(
                gene =>
                    new MissionAssignmentPairing(gene.Mission, gene.UAV.TailId, gene.TimeWindow)
            );

            Dictionary<int, Dictionary<TelemetryFields, double>> telemetryData = allUAVs.ToDictionary(
                uav => uav.TailId,
                uav => uav.TelemetryData
            );

            return new AssignmentResult(pairings, telemetryData, chromosome.FitnessScore);
        }
    }
}
