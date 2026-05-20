using Mission_Service.Models.Dto;
using Mission_Service.Models.Ro;

namespace Mission_Service.DataBase.MongoDB.Repositoreis.Interfaces
{
    public interface IAssignmentRepository
    {
        Task<bool> SaveAssignmentAsync(
            ApplyAssignmentDto applyAssignmentDto,
            CancellationToken cancellationToken = default
        );
    }
}
