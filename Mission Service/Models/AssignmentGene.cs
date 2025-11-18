namespace Mission_Service.Models
{
    public class AssignmentGene
    {
        public string MissionId { get; set; }
        public string UAVTailId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime Duration { get; set; }

        public AssignmentGene(string missionId, string uavTailId, DateTime startTime, DateTime duration)
        {
            MissionId = missionId;
            UAVTailId = uavTailId;
            StartTime = startTime;
            Duration = duration;
        }

        public AssignmentGene()
        {
        }

    }
}
