namespace Mission_Service.Models.RO
{
    public record AssignmentRequestAcceptedResponse(
        string Message,
        string RequestId,
        string StatusUrl
    );
}
