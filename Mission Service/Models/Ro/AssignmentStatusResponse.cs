namespace Mission_Service.Models.RO
{
 public record AssignmentStatusResponse(
        string RequestId,
        bool IsReady,
string Message,
        string? ResultUrl
    );
}
