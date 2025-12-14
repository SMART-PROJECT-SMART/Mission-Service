using Microsoft.AspNetCore.Mvc;
using Mission_Service.Common.Constants;
using Mission_Service.Models.choromosomes;
using Mission_Service.Models.RO;
using Mission_Service.Services.AssignmentResultManager;

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
            AssignmentChromosome? result = _assignmentResultManager.GetAndRemoveResult(assignmentId);

            if (result != null) return Ok(result);
            var notFoundResponse = new AssignmentResultNotFoundResponse(
                MissionServiceConstants.APIResponses.ASSIGNMENT_RESULT_NOT_FOUND,
                assignmentId
            );

            return NotFound(notFoundResponse);

        }

        [HttpGet("{assignmentId}/" + MissionServiceConstants.Actions.STATUS)]
        public IActionResult CheckAssignmentStatus(string assignmentId)
        {
            bool isReady = _assignmentResultManager.HasResult(assignmentId);

            string? resultUrl = isReady
                ? Url.Action(
                    nameof(GetAssignmentResult),
                    MissionServiceConstants.Controllers.ASSIGNMENT_RESULT_CONTROLLER,
                    new { assignmentId },
                    Request.Scheme
                )
                : null;

            var statusResponse = new AssignmentStatusResponse(
                assignmentId,
                isReady,
                isReady
                    ? MissionServiceConstants.APIResponses.ASSIGNMENT_RESULT_READY
                    : MissionServiceConstants.APIResponses.ASSIGNMENT_RESULT_PROCESSING,
                resultUrl
            );

            return Ok(statusResponse);
        }
    }
}
