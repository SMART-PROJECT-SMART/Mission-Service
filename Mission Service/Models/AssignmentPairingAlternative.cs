namespace Mission_Service.Models
{
    public class AssignmentPairingAlternative
    {
        public int TailId { get; set; }
        public double TotalScore { get; set; }
        public double TelemetryScore { get; set; }
        public double DistanceScore { get; set; }
        public double PriorityScore { get; set; }
        public double TypeMismatchPenalty { get; set; }
        public double ActiveMissionPenalty { get; set; }
    }
}
