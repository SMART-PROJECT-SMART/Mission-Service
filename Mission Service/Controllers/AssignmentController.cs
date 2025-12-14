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
            // Ensure RequestId is set (generate if not provided by client)
            assignmentSuggestionDto.EnsureRequestId();

            await _queue.QueueAssignmentSuggestionRequest(assignmentSuggestionDto);

            string statusUrl = Url.Action(
                nameof(AssignmentResultController.CheckAssignmentStatus),
                MissionServiceConstants.Controllers.ASSIGNMENT_RESULT_CONTROLLER,
                new { requestId = assignmentSuggestionDto.RequestId },
                Request.Scheme
            );

            var response = new AssignmentRequestAcceptedResponse(
                MissionServiceConstants.APIResponses.ASSIGNMENT_REQUEST_ACCEPTED,
                assignmentSuggestionDto.RequestId!,
                statusUrl!
            );

            return Accepted(response);
        }
    }
}
