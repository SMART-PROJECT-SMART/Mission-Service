using Core.Models;
using Microsoft.Extensions.Logging;
using Mission_Service.Common.Constants;
using Mission_Service.DataBase.MongoDB.Entities;
using Mission_Service.DataBase.MongoDB.Services;
using Mission_Service.Models.Dto;
using Mission_Service.Services.MissionExecutor.Interfaces;
using Mission_Service.Services.UAVStatusService.Interfaces;

namespace Mission_Service.Services.MissionExecutor
{
    public class MissionExecutor : IMissionExecutor
    {
        private readonly HttpClient _simulatorHttpClient;
        private readonly IUAVStatusService _uavStatusService;
        private readonly IAssignmentDBService _assignmentDbService;
        private readonly ILogger<MissionExecutor> _logger;

        public MissionExecutor(
            IHttpClientFactory httpClientFactory,
            IUAVStatusService uavStatusService,
            IAssignmentDBService assignmentDbService,
            ILogger<MissionExecutor> logger
        )
        {
            _simulatorHttpClient = httpClientFactory.CreateClient(
                MissionServiceConstants.HttpClients.SIMULATOR_CLIENT
            );
            _uavStatusService = uavStatusService;
            _assignmentDbService = assignmentDbService;
            _logger = logger;
        }

        public async Task<bool> ApplyAndExecuteAssignmentAsync(
            ApplyAssignmentDto applyAssignmentDto,
            CancellationToken cancellationToken = default
        )
        {
            bool isCreated = await _assignmentDbService.CreateAssignmentAsync(
                applyAssignmentDto,
                cancellationToken
            );

            if (!isCreated)
            {
                return false;
            }

            await ExecuteMissionsAsync(applyAssignmentDto.ActualAssignments, cancellationToken);

            return true;
        }

        public async Task ExecuteMissionsAsync(
            IEnumerable<MissionToUavAssignment> missionAssignments,
            CancellationToken cancellationToken = default
        )
        {
            IEnumerable<Task> missionTasks = missionAssignments.Select(assignment =>
                ExecuteSingleMissionAsync(assignment, cancellationToken)
            );

            await Task.WhenAll(missionTasks);
        }

        private async Task ExecuteSingleMissionAsync(
            MissionToUavAssignment assignment,
            CancellationToken cancellationToken = default
        )
        {
            SimulateMissionRequest request = new SimulateMissionRequest
            {
                TailId = assignment.UavTailId,
                Destination = new Location(
                    assignment.Mission.Location.Latitude,
                    assignment.Mission.Location.Longitude,
                    assignment.Mission.Location.Altitude
                ),
                MissionId = assignment.Mission.Id,
            };

            try
            {
                HttpResponseMessage response = await _simulatorHttpClient.PostAsJsonAsync(
                    MissionServiceConstants.SimulatorEndpoints.SIMULATE,
                    request,
                    cancellationToken
                );

                if (response.IsSuccessStatusCode)
                {
                    _uavStatusService.SetActiveMission(assignment.UavTailId, assignment.Mission);
                    return;
                }

                _logger.LogWarning(
                    "Simulator rejected mission dispatch. TailId: {TailId}, MissionId: {MissionId}, StatusCode: {StatusCode}",
                    assignment.UavTailId,
                    assignment.Mission.Id,
                    response.StatusCode
                );
            }
            catch (HttpRequestException exception)
            {
                _logger.LogWarning(
                    exception,
                    "Simulator call failed during mission dispatch. TailId: {TailId}, MissionId: {MissionId}",
                    assignment.UavTailId,
                    assignment.Mission.Id
                );
            }
            catch (TaskCanceledException exception)
            {
                _logger.LogWarning(
                    exception,
                    "Simulator call timed out during mission dispatch. TailId: {TailId}, MissionId: {MissionId}",
                    assignment.UavTailId,
                    assignment.Mission.Id
                );
            }
        }
    }
}
