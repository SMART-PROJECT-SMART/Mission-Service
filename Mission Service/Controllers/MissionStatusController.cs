using Microsoft.AspNetCore.Mvc;
using Mission_Service.Common.Constants;
using Mission_Service.Models;
using Mission_Service.Services.UAVStatusService.Interfaces;

namespace Mission_Service.Controllers
{
    [ApiController]
    [Route("api/mission-status")]
    public class MissionStatusController : ControllerBase
    {
        private readonly IUAVStatusService _uavStatusService;

        public MissionStatusController(IUAVStatusService uavStatusService)
        {
            _uavStatusService = uavStatusService;
        }

        [HttpGet("active-mission/{tailId}")]
        public IActionResult GetActiveMission(int tailId)
        {
            Mission? activeMission = _uavStatusService.GetActiveMission(tailId);

            if (activeMission == null)
            {
                string message = string.Format(
                    MissionServiceConstants.APIResponses.ACTIVE_MISSION_NOT_FOUND,
                    tailId
                );
                return NotFound(new { Message = message });
            }

            return Ok(activeMission);
        }

        [HttpPost("mission-completed/{tailId}")]
        public IActionResult MissionCompleted(int tailId)
        {
            _uavStatusService.ClearActiveMission(tailId);
            return Ok();
        }
    }
}
