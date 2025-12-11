namespace Mission_Service.Models.Dto
{
    public class AssignmentSuggestionDto
    {
        public string? RequestId { get; set; }
        public List<Mission> Missions { get; set; }
        public List<UAV> UAVs { get; set; }

        public AssignmentSuggestionDto() { }

        public AssignmentSuggestionDto(List<Mission> missions, List<UAV> uavs)
           : this(null, missions, uavs)
    {
        }

        public AssignmentSuggestionDto(string? requestId, List<Mission> missions, List<UAV> uavs)
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
