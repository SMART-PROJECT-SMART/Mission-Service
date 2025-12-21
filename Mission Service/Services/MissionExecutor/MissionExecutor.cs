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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUAVStatusService _uavStatusService;

        public MissionExecutor(
            IHttpClientFactory httpClientFactory,
            IUAVStatusService uavStatusService
        )
        {
            _httpClientFactory = httpClientFactory;
            _uavStatusService = uavStatusService;
        }

        public async Task ExecuteMissionsAsync(
            IEnumerable<MissionToUavAssignment> missionAssignments
        )
        {
            HttpClient simulatorHttpClient = CreateSimulatorClient();

            foreach (MissionToUavAssignment assignment in missionAssignments)
            {
                await ExecuteSingleMissionAsync(simulatorHttpClient, assignment);
            }
        }

        private HttpClient CreateSimulatorClient()
        {
            return _httpClientFactory.CreateClient(
                MissionServiceConstants.HttpClients.SIMULATOR_CLIENT
            );
        }

        private async Task ExecuteSingleMissionAsync(
            HttpClient simulatorClient,
            MissionToUavAssignment assignment
        )
        {
            SimulateMissionRequest request = BuildSimulationRequest(assignment);

            SetUavActiveMission(assignment);

            await SendToSimulator(simulatorClient, request);
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

        private void SetUavActiveMission(MissionToUavAssignment assignment)
        {
            _uavStatusService.SetActiveMission(assignment.UavTailId, assignment.Mission);
        }

        private static async Task SendToSimulator(
            HttpClient simulatorClient,
            SimulateMissionRequest request
        )
        {
            await simulatorClient.PostAsJsonAsync(
                MissionServiceConstants.SimulatorEndpoints.SIMULATE,
                request
            );
        }
    }
}
