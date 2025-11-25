using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mission_Service.Common.Constants;
using Mission_Service.Models.Dto;
using Mission_Service.Services.Assignment_Request_Queue;

namespace Mission_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentSuggestionRequestQueue _requestQueue;

        public AssignmentController(IAssignmentSuggestionRequestQueue requestQueue)
        {
            _requestQueue = requestQueue;
        }

        [HttpPost("create-assignment-suggestion")]
        public async Task<IActionResult> CreateAssignmentSuggestion(AssignmentSuggestionDto assignmentSuggestionDto)
        {
            await _requestQueue.QueueAssignmentSuggestionRequest(assignmentSuggestionDto);
            return Ok(MissionServiceConstants.APIResponses.CREATE_ASSIGNMENT_RECIVED);
        }
    }
}
