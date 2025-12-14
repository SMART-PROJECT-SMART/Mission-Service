namespace Mission_Service.Models.Dto
{
    public class AssignmentSuggestionDto
    {
        public string? AssignmentId { get; set; }
        public IReadOnlyCollection<Mission> Missions { get; set; }
        public IReadOnlyCollection<UAV> UAVs { get; set; }

        public AssignmentSuggestionDto() { }

        public AssignmentSuggestionDto(IReadOnlyCollection<Mission> missions, IReadOnlyCollection<UAV> uavs)
            : this(null, missions, uavs) { }

        public AssignmentSuggestionDto(string? assignmentId, IReadOnlyCollection<Mission> missions, IReadOnlyCollection<UAV> uavs)
        {
            AssignmentId = assignmentId;
            Missions = missions;
            UAVs = uavs;
        }

        public void EnsureAssignmentId()
        {
            if (string.IsNullOrWhiteSpace(AssignmentId))
            {
                AssignmentId = Guid.NewGuid().ToString();
            }
        }
    }
}
