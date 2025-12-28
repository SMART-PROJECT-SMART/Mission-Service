using Core.Common.Enums;
using Mission_Service.Models;
using Mission_Service.Models.Dto;
using Mission_Service.Services.UAVStatusService.Interfaces;

namespace Mission_Service.Extensions
{
    public static class UAVTelemetryDataDtoExtensions
    {
        public static IReadOnlyCollection<UAV> ToUAVCollection(
            this IEnumerable<UAVTelemetryDataDto> uavDataCollection,
            IUAVStatusService uavStatusService
        )
        {
            List<UAV> convertedUAVs = new List<UAV>();

            foreach (UAVTelemetryDataDto uavData in uavDataCollection)
            {
                UAVType detectedUAVType = uavStatusService.DetermineUAVType(uavData.TelemetryData);

                UAV constructedUAV = new UAV(
                    uavData.TailId,
                    detectedUAVType,
                    uavData.TelemetryData
                );
                convertedUAVs.Add(constructedUAV);
            }

            return convertedUAVs;
        }
    }
}
