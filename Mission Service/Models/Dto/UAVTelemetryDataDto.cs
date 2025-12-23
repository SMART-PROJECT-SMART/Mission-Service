using Core.Common.Enums;

namespace Mission_Service.Models.Dto
{
    public class UAVTelemetryDataDto
    {
        public int TailId { get; set; }
        public Dictionary<TelemetryFields, double> TelemetryData { get; set; }

        public UAVTelemetryDataDto(
            int tailId,
            Dictionary<TelemetryFields, double> telemetryData
        )
        {
            TailId = tailId;
            TelemetryData = telemetryData;
        }
    }
}
