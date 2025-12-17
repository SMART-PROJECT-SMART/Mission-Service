using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mission_Service.Common.Constants;
using Mission_Service.DataBase.MongoDB.Services;
using Mission_Service.Models.Dto;
using Mission_Service.Models.RO;
using Mission_Service.Services.AssignmentRequestQueue.Interfaces;
using Mission_Service.Services.AssignmentResultManager.Interfaces;
using Quartz;

namespace Mission_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentSuggestionQueue _queue;
        private readonly IAssignmentResultManager _assignmentResultManager;
        private readonly IAssignmentService _assignmentService;
        private readonly ISchedulerFactory _schedulerFactory;

        public AssignmentController(
            IAssignmentSuggestionQueue queue,
            IAssignmentResultManager assignmentResultManager,
            IAssignmentService assignmentService,
            ISchedulerFactory schedulerFactory
        )
        {
            _queue = queue;
            _assignmentResultManager = assignmentResultManager;
            _assignmentService = assignmentService;
            _schedulerFactory = schedulerFactory;
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

            var scheduler = await _schedulerFactory.GetScheduler();

            foreach (var assignment in applyAssignmentDto.ActualAssignments)
            {
                var job = JobBuilder.Create<Services.Jobs.MissionExecutorJob>()
                    .WithIdentity(assignment.Mission.Id)
                    .UsingJobData(MissionServiceConstants.MissionExecution.MISSION_ID_KEY, assignment.Mission.Id)
                    .UsingJobData(MissionServiceConstants.MissionExecution.TAIL_ID_KEY, assignment.UavTailId)
                    .UsingJobData(MissionServiceConstants.MissionExecution.LATITUDE_KEY, assignment.Mission.Location.Latitude)
                    .UsingJobData(MissionServiceConstants.MissionExecution.LONGITUDE_KEY, assignment.Mission.Location.Longitude)
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity($"{MissionServiceConstants.MissionExecution.TRIGGER_PREFIX}{assignment.Mission.Id}")
                    .StartNow()
                    .Build();

                await scheduler.ScheduleJob(job, trigger);
            }

            return CreatedAtAction(nameof(ApplyAssignment), applyAssignmentDto);
        }
    }
}
