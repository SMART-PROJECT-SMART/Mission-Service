using System.ComponentModel.DataAnnotations;
using Mission_Service.Common.Constants;

namespace Mission_Service.Models.Dto
{
    public class AssignmentSuggestionDto
    {
        [Required(ErrorMessage = MissionServiceConstants.ValidationMessages.ASSIGNMENT_ID_REQUIRED)]
        public string AssignmentId { get; set; }

        [Required(ErrorMessage = MissionServiceConstants.ValidationMessages.MISSIONS_REQUIRED)]
        [MinLength(
            1,
            ErrorMessage = MissionServiceConstants.ValidationMessages.MISSIONS_MIN_LENGTH
        )]
        public IReadOnlyCollection<Mission> Missions { get; set; }

        public AssignmentSuggestionDto()
        {
            AssignmentId = Guid.NewGuid().ToString();
            Missions = Array.Empty<Mission>();
        }

        public AssignmentSuggestionDto(IReadOnlyCollection<Mission> missions)
            : this()
        {
            Missions = missions;
        }
    }
}
