using Mission_Service.Common.Constants;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;
using Mission_Service.Models.Dto;
using Mission_Service.Services.AssignmentRequestQueue;
using Mission_Service.Services.AssignmentResultManager;
using Mission_Service.Services.GeneticAssignmentAlgorithm.MainAlgorithm;
using Mission_Service.Common.Enums;
using Mission_Service.Services.UAVStatusService;
using Core.Common.Enums;

namespace Mission_Service.Services.AssignmentSuggestionWorker
{
    public class AssignmentSuggestionWorker : BackgroundService
    {
        private readonly IAssignmentSuggestionQueue _assignmentSuggestionQueue;
        private readonly IAssignmentAlgorithm _assignmentAlgorithm;
        private readonly IAssignmentResultManager _assignmentResultManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUAVStatusService _uavStatusService;

        public AssignmentSuggestionWorker(
            IAssignmentSuggestionQueue assignmentSuggestionQueue,
            IAssignmentAlgorithm assignmentAlgorithm,
            IAssignmentResultManager assignmentResultManager,
            IHttpClientFactory httpClientFactory,
            IUAVStatusService uavStatusService
        )
        {
            _assignmentSuggestionQueue = assignmentSuggestionQueue;
            _assignmentAlgorithm = assignmentAlgorithm;
            _assignmentResultManager = assignmentResultManager;
            _httpClientFactory = httpClientFactory;
            _uavStatusService = uavStatusService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (
                AssignmentSuggestionDto request in _assignmentSuggestionQueue.AssignmentSuggestionReader.ReadAllAsync(
                    stoppingToken
                )
            )
            {
                _assignmentResultManager.UpdateStatus(request.AssignmentId, AssignmentStatus.Processing);

                IReadOnlyCollection<UAV> uavs = await FetchUAVsFromLTS(stoppingToken);

                AssignmentResult assignmentResult = _assignmentAlgorithm.PreformAssignmentAlgorithm(
                    request.Missions,
                    uavs
                );

                AssignmentChromosome bestResult = assignmentResult.Assignments.FirstOrDefault();

                if (bestResult != null)
                {
                    _assignmentResultManager.StoreResult(request.AssignmentId, bestResult);
                }
            }
        }

        private async Task<IReadOnlyCollection<UAV>> FetchUAVsFromLTS(CancellationToken cancellationToken)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient(
                MissionServiceConstants.HttpClients.LTS_HTTP_CLIENT
            );

            HttpResponseMessage response = await httpClient.GetAsync(
                MissionServiceConstants.LTSEndpoints.ALL_UAV_TELEMETRY,
                cancellationToken
            );

            response.EnsureSuccessStatusCode();

            IEnumerable<(int TailId, IEnumerable<KeyValuePair<TelemetryFields, double>> TelemetryData)>? telemetryData =
                await response.Content.ReadFromJsonAsync<IEnumerable<(int, IEnumerable<KeyValuePair<TelemetryFields, double>>)>>(
                    cancellationToken
                );

            return ConvertTelemetryToUAVs(
                telemetryData ?? Enumerable.Empty<(int, IEnumerable<KeyValuePair<TelemetryFields, double>>)>()
            );
        }

        private IReadOnlyCollection<UAV> ConvertTelemetryToUAVs(
            IEnumerable<(int TailId, IEnumerable<KeyValuePair<TelemetryFields, double>> TelemetryData)> telemetryData
        )
        {
            List<UAV> uavs = new List<UAV>();

            foreach ((int tailId, IEnumerable<KeyValuePair<TelemetryFields, double>> data) in telemetryData)
            {
                Dictionary<TelemetryFields, double> telemetryDict = data.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value
                );

                UAVType uavType = _uavStatusService.DetermineUAVType(telemetryDict);
                Mission? activeMission = _uavStatusService.IsInActiveMission(telemetryDict)
                    ? _uavStatusService.GetActiveMission(tailId)
                    : null;

                UAV uav = new UAV(tailId, uavType, telemetryDict, activeMission);
                uavs.Add(uav);
            }

            return uavs;
        }
    }
}
