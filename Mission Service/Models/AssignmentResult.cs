using Core.Common.Enums;

namespace Mission_Service.Models
{
    public class AssignmentResult
    {
        public IEnumerable<MissionAssignmentPairing> Pairings { get; set; }
        public Dictionary<int, Dictionary<TelemetryFields, double>> UAVTelemetryData { get; set; }
        public double FitnessScore { get; set; }
        public AssignmentFitnessBreakdown FitnessBreakdown { get; set; }
        public List<AssignmentPairingInsight> PairingInsights { get; set; }

        public AssignmentResult(
            IEnumerable<MissionAssignmentPairing> pairings,
            Dictionary<int, Dictionary<TelemetryFields, double>> uavTelemetryData,
            double fitnessScore,
            AssignmentFitnessBreakdown fitnessBreakdown,
            IEnumerable<AssignmentPairingInsight> pairingInsights
        )
        {
            Pairings = pairings;
            UAVTelemetryData = uavTelemetryData;
            FitnessScore = fitnessScore;
            FitnessBreakdown = fitnessBreakdown;
            PairingInsights = pairingInsights.ToList();
        }

        public AssignmentResult()
        {
            Pairings = new List<MissionAssignmentPairing>();
            UAVTelemetryData = new Dictionary<int, Dictionary<TelemetryFields, double>>();
            FitnessScore = 0;
            FitnessBreakdown = new AssignmentFitnessBreakdown();
            PairingInsights = new List<AssignmentPairingInsight>();
        }
    }
}
