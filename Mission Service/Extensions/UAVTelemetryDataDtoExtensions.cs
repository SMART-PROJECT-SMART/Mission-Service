using Core.Common.Enums;
using Mission_Service.Models;
using Mission_Service.Models.Dto;

namespace Mission_Service.Extensions
{
    public static class UAVTelemetryDataDtoExtensions
    {
        public static IReadOnlyCollection<UAV> ToUAVCollection(
            this IEnumerable<UAVTelemetryDataDto> uavDataCollection
        )
        {
            List<UAV> convertedUAVs = new List<UAV>();

            foreach (UAVTelemetryDataDto uavData in uavDataCollection)
            {
                UAV constructedUAV = new UAV(
                    uavData.TailId,
                    uavData.UavType,
                    uavData.TelemetryData
                );
                convertedUAVs.Add(constructedUAV);
            }

            return convertedUAVs;
        }
    }
}
