namespace Mission_Service.Models
{
    public struct TimeWindow
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public TimeWindow(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public TimeWindow()
        {
        }
    }
}
