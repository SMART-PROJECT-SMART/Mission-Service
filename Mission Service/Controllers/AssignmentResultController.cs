using Microsoft.AspNetCore.Mvc;
using Mission_Service.Common.Constants;
using Mission_Service.Common.Enums;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;
using Mission_Service.Models.RO;
using Mission_Service.Services.AssignmentResultManager.Interfaces;

namespace Mission_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentResultController : ControllerBase
    {
        private readonly IAssignmentResultManager _assignmentResultManager;

        public AssignmentResultController(IAssignmentResultManager assignmentResultManager)
        {
            _assignmentResultManager = assignmentResultManager;
        }

        [HttpGet("{assignmentId}")]
        public IActionResult GetAssignmentResult(string assignmentId)
        {
            AssignmentChromosome? result = _assignmentResultManager.GetAndRemoveResult(
                assignmentId
            );

            if (result != null)
                return Ok(result);

            AssignmentResultNotFoundResponse notFoundResponse = new AssignmentResultNotFoundResponse(
                MissionServiceConstants.APIResponses.ASSIGNMENT_RESULT_NOT_FOUND,
                assignmentId
            );

            return NotFound(notFoundResponse);
        }

        [HttpGet("{assignmentId}/" + MissionServiceConstants.Actions.STATUS)]
        public IActionResult CheckAssignmentStatus(string assignmentId)
        {
            AssignmentExecution? execution = _assignmentResultManager.GetExecution(assignmentId);

            if (execution == null)
            {
                AssignmentResultNotFoundResponse notFoundResponse = new AssignmentResultNotFoundResponse(
                    MissionServiceConstants.APIResponses.ASSIGNMENT_RESULT_NOT_FOUND,
                    assignmentId
                );
                return NotFound(notFoundResponse);
            }

            string? resultUrl = execution.Status == AssignmentStatus.Completed
                ? Url.Action(
                    nameof(GetAssignmentResult),
                    MissionServiceConstants.Controllers.ASSIGNMENT_RESULT_CONTROLLER,
                    new { assignmentId },
                    Request.Scheme
                )
                : null;

            AssignmentStatusResponse statusResponse = new AssignmentStatusResponse(
                assignmentId,
                execution.Status.ToString(),
                GetStatusMessage(execution.Status),
                resultUrl
            );

            return Ok(statusResponse);
        }

        private string GetStatusMessage(AssignmentStatus status) =>
            status switch
            {
                AssignmentStatus.Pending => MissionServiceConstants.APIResponses.ASSIGNMENT_PENDING,
                AssignmentStatus.Processing => MissionServiceConstants
                    .APIResponses
                    .ASSIGNMENT_PROCESSING,
                AssignmentStatus.Completed => MissionServiceConstants
                    .APIResponses
                    .ASSIGNMENT_COMPLETED,
                _ => MissionServiceConstants.APIResponses.ASSIGNMENT_PROCESSING,
            };
    }
}
