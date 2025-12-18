using Core.Common.Enums;
using Mission_Service.Common.Constants;
using Mission_Service.Common.Enums;
using Mission_Service.Models;
using Mission_Service.Services.UAVFetcher.Interfaces;
using Mission_Service.Services.UAVStatusService.Interfaces;

namespace Mission_Service.Services.UAVFetcher
{
    public class UAVFetcher : IUAVFetcher
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUAVStatusService _uavStatusService;

        public UAVFetcher(IHttpClientFactory httpClientFactory, IUAVStatusService uavStatusService)
        {
            _httpClientFactory = httpClientFactory;
            _uavStatusService = uavStatusService;
        }

        public async Task<IReadOnlyCollection<UAV>> FetchUAVsAsync(
            CancellationToken cancellationToken
        )
        {
            IEnumerable<(
                int TailId,
                IEnumerable<KeyValuePair<TelemetryFields, double>> TelemetryData
            )> uavDataCollection = await FetchUAVFromLTS(cancellationToken);

            return ConvertUAVDataToUAVObject(uavDataCollection);
        }

        private async Task<
            IEnumerable<(
                int TailId,
                IEnumerable<KeyValuePair<TelemetryFields, double>> TelemetryData
            )>
        > FetchUAVFromLTS(CancellationToken cancellationToken)
        {
            HttpClient ltsHttpClient = _httpClientFactory.CreateClient(
                MissionServiceConstants.HttpClients.LTS_HTTP_CLIENT
            );

            HttpResponseMessage telemetryResponse = await ltsHttpClient.GetAsync(
                MissionServiceConstants.LTSEndpoints.ALL_UAV_TELEMETRY,
                cancellationToken
            );

            telemetryResponse.EnsureSuccessStatusCode();

            IEnumerable<(
                int TailId,
                IEnumerable<KeyValuePair<TelemetryFields, double>> TelemetryData
            )>? rawTelemetryData = await telemetryResponse.Content.ReadFromJsonAsync<
                IEnumerable<(int, IEnumerable<KeyValuePair<TelemetryFields, double>>)>
            >(cancellationToken);

            return rawTelemetryData
                ?? Enumerable.Empty<(int, IEnumerable<KeyValuePair<TelemetryFields, double>>)>();
        }

        private IReadOnlyCollection<UAV> ConvertUAVDataToUAVObject(
            IEnumerable<(
                int TailId,
                IEnumerable<KeyValuePair<TelemetryFields, double>> TelemetryData
            )> uavDataCollection
        )
        {
            List<UAV> convertedUAVs = new List<UAV>();

            foreach (
                (
                    int uavTailId,
                    IEnumerable<KeyValuePair<TelemetryFields, double>> telemetryFields
                ) in uavDataCollection
            )
            {
                Dictionary<TelemetryFields, double> telemetryDictionary =
                    telemetryFields.ToDictionary(field => field.Key, field => field.Value);

                UAVType detectedUAVType = _uavStatusService.DetermineUAVType(telemetryDictionary);
                Mission? uavActiveMission = _uavStatusService.GetActiveMission(uavTailId);

                UAV constructedUAV = new UAV(
                    uavTailId,
                    detectedUAVType,
                    telemetryDictionary,
                    uavActiveMission
                );
                convertedUAVs.Add(constructedUAV);
            }

            return convertedUAVs;
        }
    }
}
