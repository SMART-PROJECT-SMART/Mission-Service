using Mission_Service.DataBase.MongoDB.Entities;

namespace Mission_Service.Services.Quartz.MissionScheduler.Interfaces
{
    public interface IMissionScheduler
    {
        Task ScheduleMissionsAsync(IEnumerable<MissionToUavAssignment> missionAssignments);
    }
}
