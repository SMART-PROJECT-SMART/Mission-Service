namespace Mission_Service.Models
{
    public class AssignmentExplainability
    {
        public AssignmentFitnessBreakdown FitnessBreakdown { get; set; } = new();
        public List<AssignmentPairingInsight> PairingInsights { get; set; } = new();
    }
}
