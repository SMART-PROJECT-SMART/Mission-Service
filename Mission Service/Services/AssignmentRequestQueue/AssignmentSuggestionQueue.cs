using System.Threading.Channels;
using Microsoft.Extensions.Options;
using Mission_Service.Config;
using Mission_Service.Models.Dto;

namespace Mission_Service.Services.AssignmentRequestQueue
{
    public class AssignmentSuggestionQueue : IAssignmentSuggestionQueue
    {
        private readonly Channel<AssignmentSuggestionDto> _assignmentSuggestionRequestQueue;

        public AssignmentSuggestionQueue(IOptions<AssignmentRequestQueueConfiguration> queueConfig)
        {
            var options = new BoundedChannelOptions(queueConfig.Value.ChannelSize)
            {
                FullMode = BoundedChannelFullMode.Wait,
            };

            _assignmentSuggestionRequestQueue = Channel.CreateBounded<AssignmentSuggestionDto>(
                options
            );
        }

        public async Task QueueAssignmentSuggestionRequest(
            AssignmentSuggestionDto assignmentSuggestionDto
        )
        {
            await _assignmentSuggestionRequestQueue.Writer.WriteAsync(assignmentSuggestionDto);
        }

        public ChannelReader<AssignmentSuggestionDto> AssignmentSuggestionReader =>
            _assignmentSuggestionRequestQueue.Reader;
    }
}
