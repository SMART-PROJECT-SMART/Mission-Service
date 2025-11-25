using Mission_Service.Models.Dto;
using System.Threading.Channels;

namespace Mission_Service.Services.Assignment_Request_Queue
{
    public interface IAssignmentSuggestionQueue
    {
        public Task QueueAssignmentSuggestionRequest(AssignmentSuggestionDto assignmentSuggestionDto);
        public ChannelReader<AssignmentSuggestionDto> AssignmentSuggestionRequestReader { get; }
    }
}
