using Mission_Service.DataBase.MongoDB.Repositoreis.Interfaces;
using Mission_Service.Models.Dto;
using Mission_Service.Models.Ro;

namespace Mission_Service.DataBase.MongoDB.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _assignmentRepository;

        public AssignmentService(IAssignmentRepository assignmentRepository)
        {
            _assignmentRepository = assignmentRepository;
        }

        public async Task<bool> CreateAssignmentAsync(ApplyAssignmentDto applyAssignmentDto)
        {
            // No validation needed - ASP.NET Core validates automatically with [ApiController]
            return await _assignmentRepository.SaveAssignmentAsync(applyAssignmentDto);
        }

        public async Task<AssignmentRo?> GetAssignmentByIdAsync(string assignmentId)
        {
            return await _assignmentRepository.GetAssignmentByIdAsync(assignmentId);
        }

        public async Task<IEnumerable<AssignmentRo>> GetAllAssignmentsAsync(
            int skipCount = 0,
            int limitCount = 100
        )
        {
            return await _assignmentRepository.GetAllAssignmentsAsync(skipCount, limitCount);
        }

        public async Task<bool> DeleteAssignmentAsync(string assignmentId)
        {
            return await _assignmentRepository.DeleteAssignmentAsync(assignmentId);
        }
    }
}
