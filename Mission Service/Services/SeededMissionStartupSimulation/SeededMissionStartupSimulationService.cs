using Core.Models;
using Microsoft.Extensions.Hosting;
using Mission_Service.Common.Constants;
using Mission_Service.Models.Dto;
using Mission_Service.Services.UAVStatusService.Interfaces;

namespace Mission_Service.Services.SeededMissionStartupSimulation
{
    public class SeededMissionStartupSimulationService : IHostedService
    {
        private const int MAX_ATTEMPTS = 3;
        private static readonly TimeSpan RETRY_DELAY = TimeSpan.FromSeconds(2);

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUAVStatusService _uavStatusService;
        private readonly ILogger<SeededMissionStartupSimulationService> _logger;

        public SeededMissionStartupSimulationService(
            IHttpClientFactory httpClientFactory,
            IUAVStatusService uavStatusService,
            ILogger<SeededMissionStartupSimulationService> logger
        )
        {
            _httpClientFactory = httpClientFactory;
            _uavStatusService = uavStatusService;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            HttpClient simulatorClient = _httpClientFactory.CreateClient(
                MissionServiceConstants.HttpClients.SIMULATOR_CLIENT
            );

            IEnumerable<Task> dispatchTasks = _uavStatusService
                .GetAllActiveMissions()
                .Select(activeMission => DispatchWithRetriesAsync(
                    simulatorClient,
                    activeMission.TailId,
                    activeMission.Mission,
                    cancellationToken
                ));

            await Task.WhenAll(dispatchTasks);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task DispatchWithRetriesAsync(
            HttpClient simulatorClient,
            int tailId,
            Models.Mission mission,
            CancellationToken cancellationToken
        )
        {
            SimulateMissionRequest request = new SimulateMissionRequest
            {
                TailId = tailId,
                Destination = new Location(
                    mission.Location.Latitude,
                    mission.Location.Longitude,
                    mission.Location.Altitude
                ),
                MissionId = mission.Id
            };

            for (int attempt = 1; attempt <= MAX_ATTEMPTS; attempt++)
            {
                try
                {
                    HttpResponseMessage response = await simulatorClient.PostAsJsonAsync(
                        MissionServiceConstants.SimulatorEndpoints.SIMULATE,
                        request,
                        cancellationToken
                    );

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation(
                            "Seeded mission dispatched to simulator. TailId: {TailId}, MissionId: {MissionId}",
                            tailId,
                            mission.Id
                        );
                        return;
                    }

                    _logger.LogWarning(
                        "Simulator rejected seeded mission dispatch. TailId: {TailId}, MissionId: {MissionId}, Attempt: {Attempt}, StatusCode: {StatusCode}",
                        tailId,
                        mission.Id,
                        attempt,
                        response.StatusCode
                    );
                }
                catch (HttpRequestException exception)
                {
                    _logger.LogWarning(
                        exception,
                        "Seeded mission dispatch failed. TailId: {TailId}, MissionId: {MissionId}, Attempt: {Attempt}",
                        tailId,
                        mission.Id,
                        attempt
                    );
                }
                catch (TaskCanceledException exception)
                {
                    _logger.LogWarning(
                        exception,
                        "Seeded mission dispatch timed out. TailId: {TailId}, MissionId: {MissionId}, Attempt: {Attempt}",
                        tailId,
                        mission.Id,
                        attempt
                    );
                }

                if (attempt < MAX_ATTEMPTS)
                {
                    await Task.Delay(RETRY_DELAY, cancellationToken);
                }
            }
        }
    }
}
