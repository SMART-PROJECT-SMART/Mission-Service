using Mission_Service.Models.choromosomes;

namespace Mission_Service.Models
{
    public class AssignmentResult
    {
        public IEnumerable<AssignmentChromosome> Assignments { get; set; }

        public AssignmentResult(IEnumerable<AssignmentChromosome> assignments)
        {
            Assignments = assignments;
        }

        public AssignmentResult()
        {
            Assignments = Enumerable.Empty<AssignmentChromosome>();
        }
    }
}
