namespace Mission_Service.Models.RO
{
    public record AssignmentStatusResponse(
        string AssignmentId,
        bool IsReady,
        string Message,
        string? ResultUrl
    );
}
