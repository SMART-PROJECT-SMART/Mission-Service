using System.Threading.Channels;
using Mission_Service.Models.Dto;

namespace Mission_Service.Services.Assignment_Request_Queue
{
    public class AssignmentSuggestionQueue : IAssignmentSuggestionQueue
    {
        private readonly Channel<AssignmentSuggestionDto> _assignmentSuggestionRequestQueue;

        public AssignmentSuggestionQueue()
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
