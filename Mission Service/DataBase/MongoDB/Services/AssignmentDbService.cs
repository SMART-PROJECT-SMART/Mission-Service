using Mission_Service.DataBase.MongoDB.Repositoreis.Interfaces;
using Mission_Service.Models.Dto;
using Mission_Service.Models.Ro;

namespace Mission_Service.DataBase.MongoDB.Services
{
    public class AssignmentDbService : IAssignmentDBService
    {
        private readonly IAssignmentRepository _assignmentRepository;

        public AssignmentDbService(IAssignmentRepository assignmentRepository)
        {
            _assignmentRepository = assignmentRepository;
        }

        public async Task<bool> CreateAssignmentAsync(
            ApplyAssignmentDto applyAssignmentDto,
            CancellationToken cancellationToken = default
        )
        {
            // No validation needed - ASP.NET Core validates automatically with [ApiController]
            return await _assignmentRepository.SaveAssignmentAsync(
                applyAssignmentDto,
                cancellationToken
            );
        }
    }
}
