namespace Mission_Service.Models
{
    public class AssignmentPairingInsight
    {
        public string MissionId { get; set; } = string.Empty;
        public int SuggestedTailId { get; set; }
        public double TelemetryScore { get; set; }
        public double DistanceScore { get; set; }
        public double PriorityScore { get; set; }
        public double TypeMismatchPenalty { get; set; }
        public double ActiveMissionPenalty { get; set; }
        public double TotalScore { get; set; }
        public List<AssignmentPairingAlternative> Alternatives { get; set; } = new();
    }
}
