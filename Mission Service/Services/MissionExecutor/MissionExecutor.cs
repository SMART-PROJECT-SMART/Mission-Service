using Core.Models;
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

        public MissionExecutor(
            IHttpClientFactory httpClientFactory,
            IUAVStatusService uavStatusService,
            IAssignmentDBService assignmentDbService
        )
        {
            _simulatorHttpClient = httpClientFactory.CreateClient(
                MissionServiceConstants.HttpClients.SIMULATOR_CLIENT
            );
            _uavStatusService = uavStatusService;
            _assignmentDbService = assignmentDbService;
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

            _uavStatusService.SetActiveMission(assignment.UavTailId, assignment.Mission);

            await _simulatorHttpClient.PostAsJsonAsync(
                MissionServiceConstants.SimulatorEndpoints.SIMULATE,
                request,
                cancellationToken
            );
        }
    }
}
