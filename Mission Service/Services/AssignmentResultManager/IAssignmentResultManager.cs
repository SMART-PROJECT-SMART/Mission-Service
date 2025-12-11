using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.AssignmentResultManager
{
    public interface IAssignmentResultManager
    {
        void StoreResult(string requestId, AssignmentChromosome result);
        AssignmentChromosome? GetAndRemoveResult(string requestId);
        bool HasResult(string requestId);
    }
}
