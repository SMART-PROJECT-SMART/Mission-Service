namespace Mission_Service.Models
{
    public class AssignmentGene
    {
        public Mission Mission { get; set; }
        public UAV UAV { get; set; }
        public TimeWindow TimeWindow { get; set; }

        public AssignmentGene(Mission mission, UAV uav, TimeWindow timeWindow)
        {
            Mission = mission;
            UAV = uav;
            TimeWindow = timeWindow;
        }

        public AssignmentGene() { }

        public AssignmentGene Clone()
        {
            return new AssignmentGene
            {
                Mission = Mission,
                UAV = UAV,
                TimeWindow = TimeWindow,
            };
        }
    }
}
