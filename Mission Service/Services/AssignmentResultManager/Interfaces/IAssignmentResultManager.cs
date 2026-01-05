using Mission_Service.Common.Enums;
using Mission_Service.Models;

namespace Mission_Service.Services.AssignmentResultManager.Interfaces
{
    public interface IAssignmentResultManager
    {
        string CreateExecution();
        void UpdateStatus(string assignmentId, AssignmentStatus status);
        void StoreResult(string assignmentId, AssignmentResult result);
        AssignmentExecution? GetExecution(string assignmentId);
        AssignmentResult? GetAndRemoveResult(string assignmentId);
        bool HasResult(string assignmentId);
    }
}
