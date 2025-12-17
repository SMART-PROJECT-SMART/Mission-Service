using Mission_Service.Common.Constants;
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

            object missionPayload = new
            {
                TailId = missionDataMap.GetInt(MissionServiceConstants.MissionExecution.TAIL_ID_KEY),
                Destination = new
                {
                    Latitude = missionDataMap.GetDouble(
                        MissionServiceConstants.MissionExecution.LATITUDE_KEY
                    ),
                    Longitude = missionDataMap.GetDouble(
                        MissionServiceConstants.MissionExecution.LONGITUDE_KEY
                    ),
                },
                MissionId = missionDataMap.GetString(
                    MissionServiceConstants.MissionExecution.MISSION_ID_KEY
                ),
            };

            await simulatorHttpClient.PostAsJsonAsync(
                MissionServiceConstants.SimulatorEndpoints.SIMULATE,
                missionPayload
            );
        }
    }
}
