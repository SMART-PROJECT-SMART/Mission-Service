using Mission_Service.Common.Enums;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Models
{
    public class AssignmentExecution
    {
        public AssignmentStatus Status { get; set; }
        public AssignmentChromosome? Result { get; set; }

        public AssignmentExecution()
        {
            Status = AssignmentStatus.Pending;
        }
    }
}
