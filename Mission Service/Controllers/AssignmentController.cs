using Microsoft.AspNetCore.Mvc;
using Mission_Service.Common.Constants;
using Mission_Service.DataBase.MongoDB.Services;
using Mission_Service.Models;
using Mission_Service.Models.Dto;
using Mission_Service.Models.RO;
using Mission_Service.Services.AssignmentRequestQueue.Interfaces;
using Mission_Service.Services.AssignmentResultManager.Interfaces;
using Mission_Service.Services.Quartz.MissionScheduler.Interfaces;

namespace Mission_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentSuggestionQueue _queue;
        private readonly IAssignmentResultManager _assignmentResultManager;
        private readonly IAssignmentDBService _assignmentDbService;
        private readonly IMissionScheduler _missionScheduler;

        public AssignmentController(
            IAssignmentSuggestionQueue queue,
            IAssignmentResultManager assignmentResultManager,
            IAssignmentDBService assignmentDbService,
            IMissionScheduler missionScheduler
        )
        {
            _queue = queue;
            _assignmentResultManager = assignmentResultManager;
            _assignmentDbService = assignmentDbService;
            _missionScheduler = missionScheduler;
        }

        [HttpPost(MissionServiceConstants.Actions.CREATE_ASSIGNMENT_SUGGESTION)]
        public async Task<IActionResult> CreateAssignmentSuggestion(
            AssignmentSuggestionDto assignmentSuggestionDto
        )
        {
            string assignmentId = Guid.NewGuid().ToString();

            _assignmentResultManager.CreateExecution(assignmentId);

            var request = new AssignmentSuggestionRequest(assignmentId, assignmentSuggestionDto);
            await _queue.QueueAssignmentSuggestionRequest(request);

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
        public async Task<IActionResult> ApplyAssignment(ApplyAssignmentDto applyAssignmentDto)
        {
            bool isCreated = await _assignmentDbService.CreateAssignmentAsync(applyAssignmentDto);

            if (!isCreated)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            await _missionScheduler.ScheduleMissionsAsync(applyAssignmentDto.ActualAssignments);

            return CreatedAtAction(nameof(ApplyAssignment), applyAssignmentDto);
        }
    }
}
