namespace Mission_Service.Models.Dto
{
    public class CreateAssignmentDto
    {
        public List<Mission> Missions { get; set; }
        public List<UAV> UAVs { get; set; }
        public CreateAssignmentDto() { }

        public CreateAssignmentDto(List<Mission> missions, List<UAV> uavs)
        {
            Missions = missions;
            UAVs = uavs;
        }
    }
}
