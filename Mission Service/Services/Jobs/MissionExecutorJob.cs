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
            var dataMap = context.JobDetail.JobDataMap;

            var client = _httpClientFactory.CreateClient(
                MissionServiceConstants.HttpClients.SIMULATOR_CLIENT
            );

            var payload = new
            {
                TailId = dataMap.GetInt(MissionServiceConstants.MissionExecution.TAIL_ID_KEY),
                Destination = new
                {
                    Latitude = dataMap.GetDouble(
                        MissionServiceConstants.MissionExecution.LATITUDE_KEY
                    ),
                    Longitude = dataMap.GetDouble(
                        MissionServiceConstants.MissionExecution.LONGITUDE_KEY
                    ),
                },
                MissionId = dataMap.GetString(
                    MissionServiceConstants.MissionExecution.MISSION_ID_KEY
                ),
            };

            await client.PostAsJsonAsync(
                MissionServiceConstants.SimulatorEndpoints.SIMULATE,
                payload
            );
        }
    }
}
