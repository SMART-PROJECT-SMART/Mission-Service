namespace Mission_Service.Models.RO
{
    public record AssignmentStatusResponse(
        string AssignmentId,
        string Status,
        string Message,
        string? ResultUrl
    );
}
