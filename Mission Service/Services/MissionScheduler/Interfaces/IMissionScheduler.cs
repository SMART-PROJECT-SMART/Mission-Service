using Mission_Service.DataBase.MongoDB.Entities;

namespace Mission_Service.Services.MissionScheduler.Interfaces
{
    public interface IMissionScheduler
    {
        Task ScheduleMissionsAsync(IEnumerable<MissionToUavAssignment> assignments);
    }
}
