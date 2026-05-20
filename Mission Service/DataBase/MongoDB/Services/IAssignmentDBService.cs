using Mission_Service.Models.Dto;
using Mission_Service.Models.Ro;

namespace Mission_Service.DataBase.MongoDB.Services
{
    public interface IAssignmentDBService
    {
        Task<bool> CreateAssignmentAsync(
            ApplyAssignmentDto applyAssignmentDto,
            CancellationToken cancellationToken = default
        );
    }
}
