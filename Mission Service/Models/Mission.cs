using Core.Common.Enums;
using Core.Models;
using Mission_Service.Common.Enums;

namespace Mission_Service.Models
{
    public class Mission
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public UAVType RequiredUAVType { get; set; }
        public MissionPriority Priority { get; set; }
        public TimeWindow TimeWindow { get; set; }
        public Location Location { get; set; }
    }
}
