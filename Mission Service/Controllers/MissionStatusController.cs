using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("mission-completed/{tailId}")]
        public IActionResult MissionCompleted(int tailId)
        {
            _uavStatusService.ClearActiveMission(tailId);
            return Ok();
        }
    }
}
