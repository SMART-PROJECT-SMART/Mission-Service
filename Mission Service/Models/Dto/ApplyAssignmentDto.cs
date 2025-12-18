using System.ComponentModel.DataAnnotations;
using Mission_Service.Common.Constants;
using Mission_Service.DataBase.MongoDB.Entities;

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
        public IEnumerable<MissionToUavAssignment> SuggestedAssignments { get; set; } =
            Array.Empty<MissionToUavAssignment>();

        [Required(
            ErrorMessage = MissionServiceConstants.ValidationMessages.ACTUAL_ASSIGNMENTS_REQUIRED
        )]
        [MinLength(
            1,
            ErrorMessage = MissionServiceConstants.ValidationMessages.ACTUAL_ASSIGNMENTS_MIN_LENGTH
        )]
        public IEnumerable<MissionToUavAssignment> ActualAssignments { get; set; } =
            Array.Empty<MissionToUavAssignment>();
    }
}
