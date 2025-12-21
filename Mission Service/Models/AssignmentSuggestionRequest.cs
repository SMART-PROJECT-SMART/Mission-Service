using Mission_Service.Models.Dto;

namespace Mission_Service.Models
{
    public class AssignmentSuggestionRequest
    {
        public string AssignmentId { get; init; }
        public AssignmentSuggestionDto Dto { get; init; }

        public AssignmentSuggestionRequest(string assignmentId, AssignmentSuggestionDto dto)
        {
            AssignmentId = assignmentId;
            Dto = dto;
        }
    }
}
