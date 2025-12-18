using Core.Models;

namespace Mission_Service.Models.Dto
{
    public class SimulateMissionRequest
    {
        public int TailId { get; set; }
        public Location Destination { get; set; } = new();
        public string MissionId { get; set; } = string.Empty;
    }
}
