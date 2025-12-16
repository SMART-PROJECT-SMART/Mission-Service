using Core.Common.Enums;
using Mission_Service.Common.Constants;
using Mission_Service.Common.Enums;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;
using Mission_Service.Models.Dto;
using Mission_Service.Services.AssignmentRequestQueue.Interfaces;
using Mission_Service.Services.AssignmentResultManager.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.MainAlgorithm.Interfaces;
using Mission_Service.Services.UAVStatusService.Interfaces;

namespace Mission_Service.Services.AssignmentSuggestionWorker
{
    public class AssignmentSuggestionWorker : BackgroundService
    {
        private readonly IAssignmentSuggestionQueue _assignmentSuggestionQueue;
        private readonly IAssignmentResultManager _assignmentResultManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUAVStatusService _uavStatusService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AssignmentSuggestionWorker(
            IAssignmentSuggestionQueue assignmentSuggestionQueue,
            IAssignmentResultManager assignmentResultManager,
            IHttpClientFactory httpClientFactory,
            IUAVStatusService uavStatusService,
            IServiceScopeFactory serviceScopeFactory
        )
        {
            _assignmentSuggestionQueue = assignmentSuggestionQueue;
            _assignmentResultManager = assignmentResultManager;
            _httpClientFactory = httpClientFactory;
            _uavStatusService = uavStatusService;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (
                AssignmentSuggestionDto request in _assignmentSuggestionQueue.AssignmentSuggestionReader.ReadAllAsync(
                    stoppingToken
                )
            )
            {
                _assignmentResultManager.UpdateStatus(
                    request.AssignmentId,
                    AssignmentStatus.Processing
                );

                IReadOnlyCollection<UAV> uavs = await FetchUAVsFromLTS(stoppingToken);

                using (IServiceScope scope = _serviceScopeFactory.CreateScope())
                {
                    IAssignmentAlgorithm assignmentAlgorithm =
                        scope.ServiceProvider.GetRequiredService<IAssignmentAlgorithm>();

                    AssignmentResult assignmentResult =
                        assignmentAlgorithm.PreformAssignmentAlgorithm(request.Missions, uavs);

                    AssignmentChromosome bestResult = assignmentResult.Assignments.FirstOrDefault();

                    _assignmentResultManager.StoreResult(request.AssignmentId, bestResult);
                }
            }
        }

        private async Task<IReadOnlyCollection<UAV>> FetchUAVsFromLTS(
            CancellationToken cancellationToken
        )
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

            IEnumerable<(
                int TailId,
                IEnumerable<KeyValuePair<TelemetryFields, double>> TelemetryData
            )> telemetryDataCollection =
                rawTelemetryData
                ?? Enumerable.Empty<(int, IEnumerable<KeyValuePair<TelemetryFields, double>>)>();

            return ConvertTelemetryToUAVs(telemetryDataCollection);
        }

        private IReadOnlyCollection<UAV> ConvertTelemetryToUAVs(
            IEnumerable<(
                int TailId,
                IEnumerable<KeyValuePair<TelemetryFields, double>> TelemetryData
            )> telemetryDataCollection
        )
        {
            List<UAV> convertedUAVs = new List<UAV>();

            foreach (
                (
                    int uavTailId,
                    IEnumerable<KeyValuePair<TelemetryFields, double>> telemetryFields
                ) in telemetryDataCollection
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
