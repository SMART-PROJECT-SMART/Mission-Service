using System.Collections.Concurrent;
using Mission_Service.Models.choromosomes;
using Mission_Service.Common.Enums;
using Mission_Service.Models;

namespace Mission_Service.Services.AssignmentResultManager
{
    public class AssignmentResultManager : IAssignmentResultManager
    {
        private readonly ConcurrentDictionary<string, AssignmentExecution> _assignmentExecutions;

        public AssignmentResultManager()
        {
            _assignmentExecutions = new ConcurrentDictionary<string, AssignmentExecution>();
        }

        public void CreateExecution(string assignmentId)
        {
            var execution = new AssignmentExecution(assignmentId);
            _assignmentExecutions.TryAdd(assignmentId, execution);
        }

        public void UpdateStatus(string assignmentId, AssignmentStatus status)
        {
            if (_assignmentExecutions.TryGetValue(assignmentId, out var execution))
            {
                execution.Status = status;
            }
        }

        public void StoreResult(string assignmentId, AssignmentChromosome result)
        {
            if (_assignmentExecutions.TryGetValue(assignmentId, out var execution))
            {
                execution.Result = result;
                execution.Status = AssignmentStatus.Completed;
            }
        }

        public AssignmentExecution? GetExecution(string assignmentId)
        {
            _assignmentExecutions.TryGetValue(assignmentId, out var execution);
            return execution;
        }

        public AssignmentChromosome? GetAndRemoveResult(string assignmentId)
        {
            if (_assignmentExecutions.TryRemove(assignmentId, out var execution))
            {
                return execution.Result;
            }
            return null;
        }

        public bool HasResult(string assignmentId)
        {
            if (_assignmentExecutions.TryGetValue(assignmentId, out var execution))
            {
                return execution.Status == AssignmentStatus.Completed && execution.Result != null;
            }
            return false;
        }
    }
}
