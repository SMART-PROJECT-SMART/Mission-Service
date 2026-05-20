namespace Mission_Service.Models
{
    public class MissionAssignmentPairing
    {
        public Mission Mission { get; set; }
        public int TailId { get; set; }
        public TimeWindow TimeWindow { get; set; }

        public MissionAssignmentPairing(Mission mission, int tailId, TimeWindow timeWindow)
        {
            Mission = mission;
            TailId = tailId;
            TimeWindow = timeWindow;
        }

        public MissionAssignmentPairing() { }
    }
}
