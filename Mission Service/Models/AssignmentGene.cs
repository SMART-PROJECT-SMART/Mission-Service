namespace Mission_Service.Models
{
    public class AssignmentGene
    {
        public Mission Mission { get; set; }
        public UAV UAV { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime EndTime => StartTime + Duration;

        public AssignmentGene(Mission mission, UAV uav, DateTime startTime, TimeSpan duration)
        {
            Mission = mission;
            UAV = uav;
            StartTime = startTime;
            Duration = duration;
        }

        public AssignmentGene() { }
    }
}
