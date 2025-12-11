using Core.Common.Enums;
using Mission_Service.Common.Enums;

namespace Mission_Service.Models
{
    public class UAV
    {
        public int TailId { get; set; }
        public UAVType UavType { get; set; }
        public Mission? ActiveMission { get; set; }
        public Dictionary<TelemetryFields, double> TelemetryData { get; set; }

        public UAV(
            int tailId,
            UAVType uavType,
            Dictionary<TelemetryFields, double> telemetryData,
            Mission? activeMission = null
        )
        {
            TailId = tailId;
            UavType = uavType;
            TelemetryData = telemetryData;
            ActiveMission = activeMission;
        }

        public UAV() { }
    }
}
