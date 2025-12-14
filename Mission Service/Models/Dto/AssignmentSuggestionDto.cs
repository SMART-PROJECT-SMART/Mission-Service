namespace Mission_Service.Models.Dto
{
    public class AssignmentSuggestionDto
    {
        public string AssignmentId { get; set; }
        public IReadOnlyCollection<Mission> Missions { get; set; }
        public IReadOnlyCollection<UAV> UAVs { get; set; }

        public AssignmentSuggestionDto()
        {
            AssignmentId = Guid.NewGuid().ToString();
            Missions = Array.Empty<Mission>();
            UAVs = Array.Empty<UAV>();
        }

        public AssignmentSuggestionDto(
            IReadOnlyCollection<Mission> missions,
            IReadOnlyCollection<UAV> uavs
        )
            : this()
        {
            Missions = missions;
            UAVs = uavs;
        }
    }
}
