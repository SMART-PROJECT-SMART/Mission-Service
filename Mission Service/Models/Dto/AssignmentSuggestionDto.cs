namespace Mission_Service.Models.Dto
{
    public class AssignmentSuggestionDto
    {
        public string? RequestId { get; set; }
        public IReadOnlyCollection<Mission> Missions { get; set; }
        public IReadOnlyCollection<UAV> UAVs { get; set; }

        public AssignmentSuggestionDto() { }

        public AssignmentSuggestionDto(IReadOnlyCollection<Mission> missions, IReadOnlyCollection<UAV> uavs)
            : this(null, missions, uavs) { }

        public AssignmentSuggestionDto(string? requestId, IReadOnlyCollection<Mission> missions, IReadOnlyCollection<UAV> uavs)
        {
            RequestId = requestId;
            Missions = missions;
            UAVs = uavs;
        }

        public void EnsureRequestId()
        {
            if (string.IsNullOrWhiteSpace(RequestId))
            {
                RequestId = Guid.NewGuid().ToString();
            }
        }
    }
}
