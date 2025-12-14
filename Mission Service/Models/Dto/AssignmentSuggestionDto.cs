namespace Mission_Service.Models.Dto
{
    public class AssignmentSuggestionDto
    {
        public string AssignmentId { get; set; }
        public IReadOnlyCollection<Mission> Missions { get; set; }

        public AssignmentSuggestionDto()
        {
            AssignmentId = Guid.NewGuid().ToString();
            Missions = Array.Empty<Mission>();
        }

        public AssignmentSuggestionDto(IReadOnlyCollection<Mission> missions)
            : this()
        {
            Missions = missions;
        }
    }
}
