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

        public void StoreResult(string requestId, AssignmentChromosome result)
        {
            _assignmentResults.TryAdd(requestId, result);
        }

        public AssignmentChromosome? GetAndRemoveResult(string requestId)
        {
            _assignmentResults.TryRemove(requestId, out AssignmentChromosome? result);
            return result;
        }

        public bool HasResult(string requestId)
        {
            return _assignmentResults.ContainsKey(requestId);
        }
    }
}
