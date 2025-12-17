using Mission_Service.Common.Constants;
using Mission_Service.DataBase.MongoDB.Entities;
using Mission_Service.Services.Jobs;
using Mission_Service.Services.MissionScheduler.Interfaces;
using Quartz;

namespace Mission_Service.Services.MissionScheduler
{
    public class MissionScheduler : IMissionScheduler
    {
        private readonly ISchedulerFactory _schedulerFactory;

        public MissionScheduler(ISchedulerFactory schedulerFactory)
 {
            _schedulerFactory = schedulerFactory;
  }

        public async Task ScheduleMissionsAsync(IEnumerable<MissionToUavAssignment> assignments)
        {
    IScheduler scheduler = await _schedulerFactory.GetScheduler();

    foreach (MissionToUavAssignment assignment in assignments)
      {
     IJobDetail job = CreateJob(assignment);
      ITrigger trigger = CreateTrigger(assignment.Mission.Id);
     
                await scheduler.ScheduleJob(job, trigger);
     }
        }

        private static IJobDetail CreateJob(MissionToUavAssignment assignment)
        {
            return JobBuilder.Create<MissionExecutorJob>()
    .WithIdentity(assignment.Mission.Id)
            .UsingJobData(MissionServiceConstants.MissionExecution.MISSION_ID_KEY, assignment.Mission.Id)
   .UsingJobData(MissionServiceConstants.MissionExecution.TAIL_ID_KEY, assignment.UavTailId)
     .UsingJobData(MissionServiceConstants.MissionExecution.LATITUDE_KEY, assignment.Mission.Location.Latitude)
       .UsingJobData(MissionServiceConstants.MissionExecution.LONGITUDE_KEY, assignment.Mission.Location.Longitude)
     .Build();
 }

        private static ITrigger CreateTrigger(string missionId)
        {
       return TriggerBuilder.Create()
         .WithIdentity($"{MissionServiceConstants.MissionExecution.TRIGGER_PREFIX}{missionId}")
       .StartNow()
  .Build();
        }
    }
}
