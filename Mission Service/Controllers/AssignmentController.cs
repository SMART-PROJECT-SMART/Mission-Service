using Microsoft.AspNetCore.Mvc;
using Mission_Service.Common.Constants;
using Mission_Service.DataBase.MongoDB.Services;
using Mission_Service.Models;
using Mission_Service.Models.Dto;
using Mission_Service.Models.RO;
using Mission_Service.Services.AssignmentRequestQueue.Interfaces;
using Mission_Service.Services.AssignmentResultManager.Interfaces;
using Mission_Service.Services.MissionExecutor.Interfaces;

namespace Mission_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentSuggestionQueue _assignmentSuggestionQueue;
        private readonly IAssignmentResultManager _assignmentResultManager;
        private readonly IMissionExecutor _missionExecutor;

        public AssignmentController(
            IAssignmentSuggestionQueue assignmentSuggestionQueue,
            IAssignmentResultManager assignmentResultManager,
            IMissionExecutor missionExecutor
        )
        {
            _assignmentSuggestionQueue = assignmentSuggestionQueue;
            _assignmentResultManager = assignmentResultManager;
            _missionExecutor = missionExecutor;
        }

        [HttpPost(MissionServiceConstants.Actions.CREATE_ASSIGNMENT_SUGGESTION)]
        public IActionResult CreateAssignmentSuggestion(
            AssignmentSuggestionDto assignmentSuggestionDto
        )
        {
            string assignmentId = StoreRequest(assignmentSuggestionDto);

            string statusUrl = Url.Action(
                nameof(AssignmentResultController.CheckAssignmentStatus),
                MissionServiceConstants.Controllers.ASSIGNMENT_RESULT_CONTROLLER,
                new { assignmentId },
                Request.Scheme
            )!;

            var response = new AssignmentRequestAcceptedResponse(
                MissionServiceConstants.APIResponses.ASSIGNMENT_REQUEST_ACCEPTED,
                assignmentId,
                statusUrl!
            );

            return Accepted(response);
        }

        [HttpPost("apply-assignment")]
        public async Task<IActionResult> ApplyAssignment(
            ApplyAssignmentDto applyAssignmentDto,
            CancellationToken cancellationToken = default
        )
        {
            bool success = await _missionExecutor.ApplyAndExecuteAssignmentAsync(
                applyAssignmentDto,
                cancellationToken
            );

            return success
                ? CreatedAtAction(nameof(ApplyAssignment), applyAssignmentDto)
                : StatusCode(StatusCodes.Status500InternalServerError);
        }

        private string StoreRequest(AssignmentSuggestionDto assignmentSuggestionDto)
        {
            string assignmentId = _assignmentResultManager.CreateExecution();
            var request = new AssignmentSuggestionRequest(assignmentId, assignmentSuggestionDto);
            _assignmentSuggestionQueue.QueueAssignmentSuggestionRequest(request);
            return assignmentId;
        }
    }
}
