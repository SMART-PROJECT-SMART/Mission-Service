using System.ComponentModel.DataAnnotations;
using Mission_Service.Common.Constants;

namespace Mission_Service.Models.Dto
{
    public class ApplyAssignmentDto
    {
        [Required(
            ErrorMessage = MissionServiceConstants.ValidationMessages.SUGGESTED_ASSIGNMENTS_REQUIRED
        )]
        [MinLength(
            1,
            ErrorMessage = MissionServiceConstants
                .ValidationMessages
                .SUGGESTED_ASSIGNMENTS_MIN_LENGTH
        )]
        public IEnumerable<KeyValuePair<Mission, int>> SuggestedAssignments { get; set; } =
            Array.Empty<KeyValuePair<Mission, int>>();

        [Required(
            ErrorMessage = MissionServiceConstants.ValidationMessages.ACTUAL_ASSIGNMENTS_REQUIRED
        )]
        [MinLength(
            1,
            ErrorMessage = MissionServiceConstants.ValidationMessages.ACTUAL_ASSIGNMENTS_MIN_LENGTH
        )]
        public IEnumerable<KeyValuePair<Mission, int>> ActualAssignments { get; set; } =
            Array.Empty<KeyValuePair<Mission, int>>();
    }
}
