using Mission_Service.Common.Constants;
using Mission_Service.DataBase.MongoDB.Entities;
using Mission_Service.Services.Quartz.Jobs;
using Mission_Service.Services.Quartz.MissionScheduler.Interfaces;
using Quartz;

namespace Mission_Service.Services.Quartz.MissionScheduler
{
    public class MissionScheduler : IMissionScheduler
    {
        private readonly ISchedulerFactory _schedulerFactory;

        public MissionScheduler(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        public async Task ScheduleMissionsAsync(
            IEnumerable<MissionToUavAssignment> missionAssignments
        )
        {
            IScheduler quartzScheduler = await _schedulerFactory.GetScheduler();

            foreach (MissionToUavAssignment missionAssignment in missionAssignments)
            {
                IJobDetail missionExecutionJob = CreateMissionExecutionJob(missionAssignment);
                ITrigger missionExecutionTrigger = CreateScheduledTrigger(missionAssignment);

                await quartzScheduler.ScheduleJob(missionExecutionJob, missionExecutionTrigger);
            }
        }

        private static IJobDetail CreateMissionExecutionJob(
            MissionToUavAssignment missionAssignment
        )
        {
            return JobBuilder
                .Create<MissionExecutorJob>()
                .WithIdentity(missionAssignment.Mission.Id)
                .UsingJobData(
                    MissionServiceConstants.MissionExecution.MISSION_ID_KEY,
                    missionAssignment.Mission.Id
                )
                .UsingJobData(
                    MissionServiceConstants.MissionExecution.TAIL_ID_KEY,
                    missionAssignment.UavTailId
                )
                .UsingJobData(
                    MissionServiceConstants.MissionExecution.LATITUDE_KEY,
                    missionAssignment.Mission.Location.Latitude
                )
                .UsingJobData(
                    MissionServiceConstants.MissionExecution.LONGITUDE_KEY,
                    missionAssignment.Mission.Location.Longitude
                )
                .UsingJobData(
                    MissionServiceConstants.MissionExecution.ALTITUDE_KEY,
                    missionAssignment.Mission.Location.Altitude
                )
                .Build();
        }

        private static ITrigger CreateScheduledTrigger(MissionToUavAssignment missionAssignment)
        {
            string triggerIdentity =
                $"{MissionServiceConstants.MissionExecution.TRIGGER_PREFIX}{missionAssignment.Mission.Id}";

            return TriggerBuilder
                .Create()
                .WithIdentity(triggerIdentity)
                .StartAt(missionAssignment.StartTime)
                .Build();
        }
    }
}
