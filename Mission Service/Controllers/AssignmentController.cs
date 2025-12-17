using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mission_Service.Common.Constants;
using Mission_Service.DataBase.MongoDB.Services;
using Mission_Service.Models.Dto;
using Mission_Service.Models.RO;
using Mission_Service.Services.AssignmentRequestQueue.Interfaces;
using Mission_Service.Services.AssignmentResultManager.Interfaces;

namespace Mission_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentSuggestionQueue _queue;
        private readonly IAssignmentResultManager _assignmentResultManager;
        private readonly IAssignmentService _assignmentService;

        public AssignmentController(
            IAssignmentSuggestionQueue queue,
            IAssignmentResultManager assignmentResultManager,
            IAssignmentService assignmentService
        )
        {
            _queue = queue;
            _assignmentResultManager = assignmentResultManager;
            _assignmentService = assignmentService;
        }

        [HttpPost(MissionServiceConstants.Actions.CREATE_ASSIGNMENT_SUGGESTION)]
        public async Task<IActionResult> CreateAssignmentSuggestion(
            AssignmentSuggestionDto assignmentSuggestionDto
        )
        {
            _assignmentResultManager.CreateExecution(assignmentSuggestionDto.AssignmentId);

            await _queue.QueueAssignmentSuggestionRequest(assignmentSuggestionDto);

            string statusUrl = Url.Action(
                nameof(AssignmentResultController.CheckAssignmentStatus),
                MissionServiceConstants.Controllers.ASSIGNMENT_RESULT_CONTROLLER,
                new { assignmentId = assignmentSuggestionDto.AssignmentId },
                Request.Scheme
            )!;

            var response = new AssignmentRequestAcceptedResponse(
                MissionServiceConstants.APIResponses.ASSIGNMENT_REQUEST_ACCEPTED,
                assignmentSuggestionDto.AssignmentId!,
                statusUrl!
            );

            return Accepted(response);
        }

        [HttpPost("apply-assignment")]
        public async Task<IActionResult> ApplyAssignment(ApplyAssignmentDto applyAssignmentDto)
        {
            bool isCreated = await _assignmentService.CreateAssignmentAsync(applyAssignmentDto);

            if (!isCreated)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return CreatedAtAction(nameof(ApplyAssignment), applyAssignmentDto);
        }
    }
}
