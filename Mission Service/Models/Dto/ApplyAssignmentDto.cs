namespace Mission_Service.Models.Dto
{
    public class ApplyAssignmentDto
    {
        public IEnumerable<KeyValuePair<Mission, int>> SuggestedAssignments { get; set; } =
            Array.Empty<KeyValuePair<Mission, int>>();

        public IEnumerable<KeyValuePair<Mission, int>> ActualAssignments { get; set; } =
            Array.Empty<KeyValuePair<Mission, int>>();
    }
}
