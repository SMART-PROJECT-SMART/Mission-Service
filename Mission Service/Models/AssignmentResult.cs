using Mission_Service.Models.choromosomes;

namespace Mission_Service.Models
{
    public class AssignmentResult
    {
        public AssignmentChromosome Assignment { get; set; }

        public AssignmentResult(AssignmentChromosome assignment)
        {
            Assignment = assignment;
        }

        public AssignmentResult()
        {
            Assignment = new AssignmentChromosome();
        }
    }
}
