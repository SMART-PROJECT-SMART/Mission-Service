using System.Threading.Channels;
using Mission_Service.Models;

namespace Mission_Service.Services.AssignmentRequestQueue.Interfaces
{
    public interface IAssignmentSuggestionQueue
    {
        public Task QueueAssignmentSuggestionRequest(
            AssignmentSuggestionRequest assignmentSuggestionRequest
        );
        public ChannelReader<AssignmentSuggestionRequest> AssignmentSuggestionReader { get; }
    }
}
