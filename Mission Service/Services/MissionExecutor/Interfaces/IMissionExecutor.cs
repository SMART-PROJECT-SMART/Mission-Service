using Mission_Service.DataBase.MongoDB.Entities;

namespace Mission_Service.Services.MissionExecutor.Interfaces
{
    public interface IMissionExecutor
    {
        Task ExecuteMissionsAsync(
            IEnumerable<MissionToUavAssignment> missionAssignments,
            CancellationToken cancellationToken = default
        );
    }
}
