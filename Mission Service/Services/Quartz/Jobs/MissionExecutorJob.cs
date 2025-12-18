using Core.Models;
using Mission_Service.Common.Constants;
using Mission_Service.Models;
using Mission_Service.Models.Dto;
using Mission_Service.Services.UAVStatusService.Interfaces;
using Quartz;

namespace Mission_Service.Services.Quartz.Jobs
{
    public class MissionExecutorJob : IJob
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUAVStatusService _uavStatusService;

        public MissionExecutorJob(
            IHttpClientFactory httpClientFactory,
            IUAVStatusService uavStatusService
        )
        {
            _httpClientFactory = httpClientFactory;
            _uavStatusService = uavStatusService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap missionDataMap = context.JobDetail.JobDataMap;

            int tailId = missionDataMap.GetInt(
                MissionServiceConstants.MissionExecution.TAIL_ID_KEY
            );
            string missionId = missionDataMap.GetString(
                MissionServiceConstants.MissionExecution.MISSION_ID_KEY
            );

            HttpClient simulatorHttpClient = _httpClientFactory.CreateClient(
                MissionServiceConstants.HttpClients.SIMULATOR_CLIENT
            );

            SimulateMissionRequest missionRequest = new SimulateMissionRequest
            {
                TailId = tailId,
                Destination = new Location(
                    missionDataMap.GetDouble(MissionServiceConstants.MissionExecution.LATITUDE_KEY),
                    missionDataMap.GetDouble(
                        MissionServiceConstants.MissionExecution.LONGITUDE_KEY
                    ),
                    missionDataMap.GetDouble(MissionServiceConstants.MissionExecution.ALTITUDE_KEY)
                ),
                MissionId = missionId,
            };

            Mission activeMission = new Mission { Id = missionId };
            _uavStatusService.SetActiveMission(tailId, activeMission);

            await simulatorHttpClient.PostAsJsonAsync(
                MissionServiceConstants.SimulatorEndpoints.SIMULATE,
                missionRequest
            );
        }
    }
}
