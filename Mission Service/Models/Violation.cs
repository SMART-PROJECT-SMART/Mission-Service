using Mission_Service.Common.Enums;

namespace Mission_Service.Models
{
    public class Violation
    {
        public ViolationType ViolationType { get; set; }
        public SeveretyLevel SeveretyLevel { get; set; }
        public AssignmentGene AssignmentGene { get; set; }
        public Violation() { }
        public Violation(ViolationType violationType, SeveretyLevel severetyLevel, AssignmentGene assignmentGene)
        {
            ViolationType = violationType;
            SeveretyLevel = severetyLevel;
            AssignmentGene = assignmentGene;
        }
    }
}
