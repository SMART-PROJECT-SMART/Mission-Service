using Core.Common.Enums;
using Mission_Service.Common.Constants;
using Mission_Service.Common.Enums;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;
using Mission_Service.Models.Dto;
using Mission_Service.Services.AssignmentRequestQueue.Interfaces;
using Mission_Service.Services.AssignmentResultManager.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.MainAlgorithm.Interfaces;
using Mission_Service.Services.UAVTelemetryService.Interfaces;

namespace Mission_Service.Services.AssignmentSuggestionWorker
{
    public class AssignmentSuggestionWorker : BackgroundService
    {
        private readonly IAssignmentSuggestionQueue _assignmentSuggestionQueue;
        private readonly IAssignmentResultManager _assignmentResultManager;
        private readonly IUAVFetcher _iuavFetcher;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AssignmentSuggestionWorker(
            IAssignmentSuggestionQueue assignmentSuggestionQueue,
            IAssignmentResultManager assignmentResultManager,
            IUAVFetcher iuavFetcher,
            IServiceScopeFactory serviceScopeFactory
        )
        {
            _assignmentSuggestionQueue = assignmentSuggestionQueue;
            _assignmentResultManager = assignmentResultManager;
            _iuavFetcher = iuavFetcher;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (
                AssignmentSuggestionDto request in _assignmentSuggestionQueue.AssignmentSuggestionReader.ReadAllAsync(
                    stoppingToken
                )
            )
            {
                _assignmentResultManager.UpdateStatus(
                    request.AssignmentId,
                    AssignmentStatus.Processing
                );

                IReadOnlyCollection<UAV> uavs = await _iuavFetcher.FetchUAVsAsync(stoppingToken);

                AssignmentChromosome assignmentResult = ExecuteAlgorithm(request, uavs);

                _assignmentResultManager.StoreResult(request.AssignmentId, assignmentResult);
            }
        }

        private AssignmentChromosome ExecuteAlgorithm(
            AssignmentSuggestionDto request,
            IReadOnlyCollection<UAV> uavs
        )
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IAssignmentAlgorithm assignmentAlgorithm =
                scope.ServiceProvider.GetRequiredService<IAssignmentAlgorithm>();

            AssignmentResult assignmentResult = assignmentAlgorithm.PreformAssignmentAlgorithm(
                request.Missions,
                uavs
            );

            return assignmentResult.Assignments.First();
        }
    }
}
