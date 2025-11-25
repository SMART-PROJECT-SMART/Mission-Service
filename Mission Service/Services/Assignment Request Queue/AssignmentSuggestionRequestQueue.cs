using System.Threading.Channels;
using Mission_Service.Models.Dto;

namespace Mission_Service.Services.Assignment_Request_Queue
{
    public class AssignmentSuggestionRequestQueue : IAssignmentSuggestionRequestQueue
    {
        private readonly Channel<AssignmentSuggestionDto> _assignmentSuggestionRequestQueue;

        public AssignmentSuggestionRequestQueue()
        {
            _assignmentSuggestionRequestQueue = Channel.CreateUnbounded<AssignmentSuggestionDto>();
        }

        public async Task QueueAssignmentSuggestionRequest(AssignmentSuggestionDto assignmentSuggestionDto)
        {
            await _assignmentSuggestionRequestQueue.Writer.WriteAsync(assignmentSuggestionDto);
        }

        public ChannelReader<AssignmentSuggestionDto> AssignmentSuggestionRequestReader =>
            _assignmentSuggestionRequestQueue.Reader;
    }
}
