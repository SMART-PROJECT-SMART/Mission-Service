using System.Threading.Channels;
using Mission_Service.Models.Dto;

namespace Mission_Service.Services.AssignmentRequestQueue
{
    public interface IAssignmentSuggestionQueue
    {
        public Task QueueAssignmentSuggestionRequest(
            AssignmentSuggestionDto assignmentSuggestionDto
        );
        public ChannelReader<AssignmentSuggestionDto> AssignmentSuggestionReader { get; }
    }
}
