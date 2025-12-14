using Mission_Service.Models.choromosomes;
using Mission_Service.Common.Enums;
using Mission_Service.Models;

namespace Mission_Service.Services.AssignmentResultManager
{
    public interface IAssignmentResultManager
    {
        void CreateExecution(string assignmentId);
        void UpdateStatus(string assignmentId, AssignmentStatus status);
        void StoreResult(string assignmentId, AssignmentChromosome result);
        AssignmentExecution? GetExecution(string assignmentId);
        AssignmentChromosome? GetAndRemoveResult(string assignmentId);
        bool HasResult(string assignmentId);
    }
}
