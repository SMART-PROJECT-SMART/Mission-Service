namespace Mission_Service.Models
{
    public class AssignmentFitnessBreakdown
    {
        public double TelemetryScore { get; set; }
        public double DistanceScore { get; set; }
        public double PriorityScore { get; set; }
        public double TimeOverlapPenalty { get; set; }
        public double TypeMismatchPenalty { get; set; }
        public double MissionCoverageBonus { get; set; }
        public double ActiveMissionPenalty { get; set; }
        public double TotalFitnessScore { get; set; }
    }
}
