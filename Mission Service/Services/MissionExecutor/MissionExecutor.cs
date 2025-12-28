using Core.Models;
using Mission_Service.Common.Constants;
using Mission_Service.DataBase.MongoDB.Entities;
using Mission_Service.Models.Dto;
using Mission_Service.Services.MissionExecutor.Interfaces;
using Mission_Service.Services.UAVStatusService.Interfaces;

namespace Mission_Service.Services.MissionExecutor
{
    public class MissionExecutor : IMissionExecutor
    {
        private readonly HttpClient _simulatorHttpClient;
        private readonly IUAVStatusService _uavStatusService;

        public MissionExecutor(
            IHttpClientFactory httpClientFactory,
            IUAVStatusService uavStatusService
        )
        {
            _simulatorHttpClient = httpClientFactory.CreateClient(
                MissionServiceConstants.HttpClients.SIMULATOR_CLIENT
            );
            _uavStatusService = uavStatusService;
        }

        public async Task ExecuteMissionsAsync(
            IEnumerable<MissionToUavAssignment> missionAssignments
        )
        {
            foreach (MissionToUavAssignment assignment in missionAssignments)
            {
                await ExecuteSingleMissionAsync(assignment);
            }
        }

        private async Task ExecuteSingleMissionAsync(MissionToUavAssignment assignment)
        {
            SimulateMissionRequest request = BuildSimulationRequest(assignment);

            _uavStatusService.SetActiveMission(assignment.UavTailId, assignment.Mission);

            await _simulatorHttpClient.PostAsJsonAsync(
                MissionServiceConstants.SimulatorEndpoints.SIMULATE,
                request
            );
        }

        private static SimulateMissionRequest BuildSimulationRequest(
            MissionToUavAssignment assignment
        )
        {
            return new SimulateMissionRequest
            {
                TailId = assignment.UavTailId,
                Destination = new Location(
                    assignment.Mission.Location.Latitude,
                    assignment.Mission.Location.Longitude,
                    assignment.Mission.Location.Altitude
                ),
                MissionId = assignment.Mission.Id,
            };
        }
    }
}
