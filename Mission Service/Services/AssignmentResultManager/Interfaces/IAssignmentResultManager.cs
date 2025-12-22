using Mission_Service.Common.Enums;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.AssignmentResultManager.Interfaces
{
    public interface IAssignmentResultManager
    {
        string CreateExecution();
        void UpdateStatus(string assignmentId, AssignmentStatus status);
        void StoreResult(string assignmentId, AssignmentChromosome result);
        AssignmentExecution? GetExecution(string assignmentId);
        AssignmentChromosome? GetAndRemoveResult(string assignmentId);
        bool HasResult(string assignmentId);
    }
}
