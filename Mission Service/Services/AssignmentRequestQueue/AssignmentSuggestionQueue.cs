using System.Threading.Channels;
using Microsoft.Extensions.Options;
using Mission_Service.Config;
using Mission_Service.Models;
using Mission_Service.Services.AssignmentRequestQueue.Interfaces;

namespace Mission_Service.Services.AssignmentRequestQueue
{
    public class AssignmentSuggestionQueue : IAssignmentSuggestionQueue
    {
        private readonly Channel<AssignmentSuggestionRequest> _assignmentSuggestionRequestQueue;

        public AssignmentSuggestionQueue(IOptions<AssignmentRequestQueueConfiguration> queueConfig)
        {
            var options = new BoundedChannelOptions(queueConfig.Value.ChannelSize)
            {
                FullMode = BoundedChannelFullMode.Wait,
            };

            _assignmentSuggestionRequestQueue = Channel.CreateBounded<AssignmentSuggestionRequest>(
                options
            );
        }

        public async Task QueueAssignmentSuggestionRequest(
            AssignmentSuggestionRequest assignmentSuggestionRequest
        )
        {
            await _assignmentSuggestionRequestQueue.Writer.WriteAsync(assignmentSuggestionRequest);
        }

        public ChannelReader<AssignmentSuggestionRequest> AssignmentSuggestionReader =>
            _assignmentSuggestionRequestQueue.Reader;
    }
}
