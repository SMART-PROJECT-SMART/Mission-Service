using Mission_Service.Models.Dto;
using Mission_Service.Models.Ro;

namespace Mission_Service.DataBase.MongoDB.Repositoreis.Interfaces
{
    public interface IAssignmentRepository
    {
        Task<bool> SaveAssignmentAsync(ApplyAssignmentDto applyAssignmentDto);

        Task<AssignmentRo?> GetAssignmentByIdAsync(string assignmentId);

        Task<IEnumerable<AssignmentRo>> GetAllAssignmentsAsync(
            int skipCount = 0,
            int limitCount = 100
        );

        Task<bool> DeleteAssignmentAsync(string assignmentId);
    }
}
