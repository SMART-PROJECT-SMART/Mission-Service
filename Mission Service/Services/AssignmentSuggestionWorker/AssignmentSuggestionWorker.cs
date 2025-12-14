using Mission_Service.Common.Constants;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;
using Mission_Service.Models.Dto;
using Mission_Service.Services.AssignmentRequestQueue;
using Mission_Service.Services.AssignmentResultManager;
using Mission_Service.Services.GeneticAssignmentAlgorithm.MainAlgorithm;

namespace Mission_Service.Services.AssignmentSuggestionWorker
{
    public class AssignmentSuggestionWorker : BackgroundService
    {
        private readonly IAssignmentSuggestionQueue _assignmentSuggestionQueue;
        private readonly IAssignmentAlgorithm _assignmentAlgorithm;
        private readonly IAssignmentResultManager _assignmentResultManager;

        public AssignmentSuggestionWorker(
            IAssignmentSuggestionQueue assignmentSuggestionQueue,
            IAssignmentAlgorithm assignmentAlgorithm,
            IAssignmentResultManager assignmentResultManager
        )
        {
            _assignmentSuggestionQueue = assignmentSuggestionQueue;
            _assignmentAlgorithm = assignmentAlgorithm;
            _assignmentResultManager = assignmentResultManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (
                AssignmentSuggestionDto request in _assignmentSuggestionQueue.AssignmentSuggestionReader.ReadAllAsync(
                    stoppingToken
                )
            )
            {
                AssignmentResult assignmentResult = _assignmentAlgorithm.PreformAssignmentAlgorithm(
                    request.Missions,
                    request.UAVs
                );

                AssignmentChromosome bestResult = assignmentResult.Assignments.FirstOrDefault();

                if (bestResult != null)
                {
                    _assignmentResultManager.StoreResult(request.AssignmentId, bestResult);
                }
            }
        }
    }
}
