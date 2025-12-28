using Mission_Service.DataBase.MongoDB.Entities;
using Mission_Service.Models.Dto;

namespace Mission_Service.Services.MissionExecutor.Interfaces
{
    public interface IMissionExecutor
    {
        Task<bool> ApplyAndExecuteAssignmentAsync(
            ApplyAssignmentDto applyAssignmentDto,
            CancellationToken cancellationToken = default
        );

        Task ExecuteMissionsAsync(
            IEnumerable<MissionToUavAssignment> missionAssignments,
            CancellationToken cancellationToken = default
        );
    }
}
