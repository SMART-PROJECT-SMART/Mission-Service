using Mission_Service.Common.Constants;
using Mission_Service.Models;
using Mission_Service.Models.Dto;
using Mission_Service.Services.Assignment_Request_Queue;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Main_Algorithm;

namespace Mission_Service.Services.Assignment_Suggestion_Worker
{
    public class AssignmentSuggestionWorker : BackgroundService
    {
        private readonly IAssignmentSuggestionQueue _assignmentSuggestionQueue;
        private readonly IAssignmentAlgorithm _assignmentAlgorithm;
        private readonly HttpClient _httpClient;

        public AssignmentSuggestionWorker(
            IAssignmentSuggestionQueue assignmentSuggestionQueue,
            IAssignmentAlgorithm assignmentAlgorithm,
            IHttpClientFactory httpClientFactory
        )
        {
            _assignmentSuggestionQueue = assignmentSuggestionQueue;
            _assignmentAlgorithm = assignmentAlgorithm;
            _httpClient = httpClientFactory.CreateClient(
                MissionServiceConstants.HttpClients.CALLBACK_HTTP_CLIENT
            );
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
                _ = _httpClient.PostAsJsonAsync(
                    request.CallbackURL,
                    assignmentResult,
                    cancellationToken: stoppingToken
                );
            }
        }
    }
}
