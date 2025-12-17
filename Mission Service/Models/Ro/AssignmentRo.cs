using Mission_Service.Models.Entities;

namespace Mission_Service.Models.Ro
{
    public class AssignmentRo
    {
        public List<MissionToUavAssignment> SuggestedAssignments { get; set; } = new();

        public List<MissionToUavAssignment> ActualAssignments { get; set; } = new();
    }
}
