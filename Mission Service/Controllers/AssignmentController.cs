using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mission_Service.Common.Constants;
using Mission_Service.Models.Dto;
using Mission_Service.Models.RO;
using Mission_Service.Services.AssignmentRequestQueue;

namespace Mission_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentSuggestionQueue _queue;

        public AssignmentController(IAssignmentSuggestionQueue queue)
        {
            _queue = queue;
        }

        [HttpPost(MissionServiceConstants.Actions.CREATE_ASSIGNMENT_SUGGESTION)]
        public async Task<IActionResult> CreateAssignmentSuggestion(
            AssignmentSuggestionDto assignmentSuggestionDto
        )
        {
            // Ensure AssignmentId is set (generate if not provided by client)
            assignmentSuggestionDto.EnsureAssignmentId();

            await _queue.QueueAssignmentSuggestionRequest(assignmentSuggestionDto);

            string statusUrl = Url.Action(
                nameof(AssignmentResultController.CheckAssignmentStatus),
                MissionServiceConstants.Controllers.ASSIGNMENT_RESULT_CONTROLLER,
                new { assignmentId = assignmentSuggestionDto.AssignmentId },
                Request.Scheme
            );

            var response = new AssignmentRequestAcceptedResponse(
                MissionServiceConstants.APIResponses.ASSIGNMENT_REQUEST_ACCEPTED,
                assignmentSuggestionDto.AssignmentId!,
                statusUrl!
            );

            return Accepted(response);
        }
    }
}
