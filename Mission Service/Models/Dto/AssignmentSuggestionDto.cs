namespace Mission_Service.Models.Dto
{
    public class AssignmentSuggestionDto
    {
        public List<Mission> Missions { get; set; }
        public List<UAV> UAVs { get; set; }
        public string CallbackURL { get; set; }
        public AssignmentSuggestionDto() { }

        public AssignmentSuggestionDto(List<Mission> missions, List<UAV> uavs, string callbackURL)
        {
            Missions = missions;
            UAVs = uavs;
            CallbackURL = callbackURL;
        }
    }
}
