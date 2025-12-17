using Mission_Service.Models.Dto;
using Mission_Service.Models.Ro;

namespace Mission_Service.Services.Repositoreis.Interfaces
{
    public interface IAssignmentRepository
    {
        Task SaveAssignmentAsync(ApplyAssignmentDto applyAssignmentDto);
        
        Task<AssignmentRo?> GetAssignmentByIdAsync(string assignmentId);
        
        Task<IEnumerable<AssignmentRo>> GetAllAssignmentsAsync(int skip = 0, int limit = 100);
        
        Task<bool> DeleteAssignmentAsync(string assignmentId);
    }
}
