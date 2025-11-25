using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mission_Service.Common.Constants;
using Mission_Service.Models.Dto;

namespace Mission_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        public IActionResult CreateAssignmentSuggestion(CreateAssignmentDto createAssignmentDto)
        {
            return Ok(MissionServiceConstants.APIResponses.CREATE_ASSIGNMENT_RECIVED);
        }
    }
}
