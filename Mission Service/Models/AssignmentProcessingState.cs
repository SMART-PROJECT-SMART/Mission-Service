using Mission_Service.Common.Enums;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Models
{
    public class AssignmentExecution
    {
        public string AssignmentId { get; init; }
        public AssignmentStatus Status { get; set; }
        public AssignmentChromosome? Result { get; set; }

        public AssignmentExecution(string assignmentId)
        {
            AssignmentId = assignmentId;
            Status = AssignmentStatus.Pending;
        }
    }
}
