using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.AssignmentResultManager
{
    public interface IAssignmentResultManager
    {
        void StoreResult(string assignmentId, AssignmentChromosome result);
        AssignmentChromosome? GetAndRemoveResult(string assignmentId);
        bool HasResult(string assignmentId);
    }
}
