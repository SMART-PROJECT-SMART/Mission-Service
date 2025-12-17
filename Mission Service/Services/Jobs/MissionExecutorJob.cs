using Core.Models;
using Mission_Service.Common.Constants;
using Mission_Service.Models.Dto;
using Quartz;

namespace Mission_Service.Services.Jobs
{
    public class MissionExecutorJob : IJob
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MissionExecutorJob(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap missionDataMap = context.JobDetail.JobDataMap;

            HttpClient simulatorHttpClient = _httpClientFactory.CreateClient(
                MissionServiceConstants.HttpClients.SIMULATOR_CLIENT
            );

            SimulateMissionRequest missionRequest = new SimulateMissionRequest
            {
                TailId = missionDataMap.GetInt(MissionServiceConstants.MissionExecution.TAIL_ID_KEY),
                Destination = new Location(
                    missionDataMap.GetDouble(MissionServiceConstants.MissionExecution.LATITUDE_KEY),
                    missionDataMap.GetDouble(MissionServiceConstants.MissionExecution.LONGITUDE_KEY),
                    missionDataMap.GetDouble(MissionServiceConstants.MissionExecution.ALTITUDE_KEY)
                ),
                MissionId = missionDataMap.GetString(
                    MissionServiceConstants.MissionExecution.MISSION_ID_KEY
                ),
            };

            await simulatorHttpClient.PostAsJsonAsync(
                MissionServiceConstants.SimulatorEndpoints.SIMULATE,
                missionRequest
            );
        }
    }
}
