using Core.Common.Enums;

namespace Mission_Service.Models
{
    public class AssignmentResult
    {
        public IEnumerable<MissionAssignmentPairing> Pairings { get; set; }
        public Dictionary<int, Dictionary<TelemetryFields, double>> UAVTelemetryData { get; set; }
        public double FitnessScore { get; set; }

        public AssignmentResult(
            IEnumerable<MissionAssignmentPairing> pairings,
            Dictionary<int, Dictionary<TelemetryFields, double>> uavTelemetryData,
            double fitnessScore
        )
        {
            Pairings = pairings;
            UAVTelemetryData = uavTelemetryData;
            FitnessScore = fitnessScore;
        }

        public AssignmentResult()
        {
            Pairings = new List<MissionAssignmentPairing>();
            UAVTelemetryData = new Dictionary<int, Dictionary<TelemetryFields, double>>();
            FitnessScore = 0;
        }
    }
}
