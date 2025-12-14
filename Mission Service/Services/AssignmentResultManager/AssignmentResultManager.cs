using System.Collections.Concurrent;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.AssignmentResultManager
{
    public class AssignmentResultManager : IAssignmentResultManager
    {
        private readonly ConcurrentDictionary<string, AssignmentChromosome> _assignmentResults;

        public AssignmentResultManager()
        {
            _assignmentResults = new ConcurrentDictionary<string, AssignmentChromosome>();
        }

        public void StoreResult(string assignmentId, AssignmentChromosome result)
        {
            _assignmentResults.TryAdd(assignmentId, result);
        }

        public AssignmentChromosome? GetAndRemoveResult(string assignmentId)
        {
            _assignmentResults.TryRemove(assignmentId, out AssignmentChromosome? result);
            return result;
        }

        public bool HasResult(string assignmentId)
        {
            return _assignmentResults.ContainsKey(assignmentId);
        }
    }
}
