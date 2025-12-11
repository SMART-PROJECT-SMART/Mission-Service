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

        [HttpGet("{requestId}")]
        public IActionResult GetAssignmentResult(string requestId)
        {
            AssignmentChromosome? result = _assignmentResultManager.GetAndRemoveResult(requestId);

            if (result == null)
            {
                var notFoundResponse = new AssignmentResultNotFoundResponse(
                    MissionServiceConstants.APIResponses.ASSIGNMENT_RESULT_NOT_FOUND,
                    requestId
                );

                return NotFound(notFoundResponse);
            }

            return Ok(result);
        }

        [HttpGet("{requestId}/" + MissionServiceConstants.Actions.STATUS)]
        public IActionResult CheckAssignmentStatus(string requestId)
        {
            bool isReady = _assignmentResultManager.HasResult(requestId);

            string? resultUrl = isReady
                ? Url.Action(
                    nameof(GetAssignmentResult),
                    MissionServiceConstants.Controllers.ASSIGNMENT_RESULT_CONTROLLER,
                    new { requestId },
                    Request.Scheme
                )
                : null;

            var statusResponse = new AssignmentStatusResponse(
                requestId,
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
