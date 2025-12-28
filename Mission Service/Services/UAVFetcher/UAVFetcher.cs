using Core.Common.Enums;
using Mission_Service.Common.Constants;
using Mission_Service.Common.Enums;
using Mission_Service.Models;
using Mission_Service.Models.Dto;
using Mission_Service.Services.UAVFetcher.Interfaces;
using Mission_Service.Services.UAVStatusService.Interfaces;

namespace Mission_Service.Services.UAVFetcher
{
    public class UAVFetcher : IUAVFetcher
    {
        private readonly HttpClient _ltsHttpClient;
        private readonly IUAVStatusService _uavStatusService;

        public UAVFetcher(IHttpClientFactory httpClientFactory, IUAVStatusService uavStatusService)
        {
            _ltsHttpClient = httpClientFactory.CreateClient(
                MissionServiceConstants.HttpClients.LTS_HTTP_CLIENT
            );
            _uavStatusService = uavStatusService;
        }

        public async Task<IReadOnlyCollection<UAV>> FetchUAVsAsync(
            CancellationToken cancellationToken
        )
        {
            IEnumerable<UAVTelemetryDataDto> uavDataCollection = await FetchUAVFromLTS(
                cancellationToken
            );

            return ConvertUAVDataToUAVObject(uavDataCollection);
        }

        private async Task<IEnumerable<UAVTelemetryDataDto>> FetchUAVFromLTS(
            CancellationToken cancellationToken
        )
        {
            HttpResponseMessage telemetryResponse = await _ltsHttpClient.GetAsync(
                MissionServiceConstants.LTSEndpoints.ALL_UAV_TELEMETRY,
                cancellationToken
            );

            telemetryResponse.EnsureSuccessStatusCode();

            IEnumerable<UAVTelemetryDataDto>? rawTelemetryData =
                await telemetryResponse.Content.ReadFromJsonAsync<IEnumerable<UAVTelemetryDataDto>>(
                    cancellationToken
                );

            return rawTelemetryData ?? Enumerable.Empty<UAVTelemetryDataDto>();
        }

        private IReadOnlyCollection<UAV> ConvertUAVDataToUAVObject(
            IEnumerable<UAVTelemetryDataDto> uavDataCollection
        )
        {
            List<UAV> convertedUAVs = new List<UAV>();

            foreach (UAVTelemetryDataDto uavData in uavDataCollection)
            {
                UAVType detectedUAVType = _uavStatusService.DetermineUAVType(uavData.TelemetryData);

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
