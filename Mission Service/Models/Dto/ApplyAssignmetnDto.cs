namespace Mission_Service.Models.Dto
{
    public class ApplyAssignmetnDto
    {
        IEnumerable<KeyValuePair<Mission,UAV>> Assignments { get; set; }

        public ApplyAssignmetnDto(IEnumerable<KeyValuePair<Mission, UAV>> assignments)
        {
            Assignments = assignments;
        }
    }
}
