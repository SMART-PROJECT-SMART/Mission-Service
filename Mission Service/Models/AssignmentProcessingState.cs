using Mission_Service.Common.Enums;

namespace Mission_Service.Models
{
    public class AssignmentExecution
    {
        public AssignmentStatus Status { get; set; }
        public AssignmentResult? Result { get; set; }

        public AssignmentExecution()
        {
            Status = AssignmentStatus.Pending;
        }
    }
}
