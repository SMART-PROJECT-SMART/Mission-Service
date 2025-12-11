namespace Mission_Service.Config
{
    public class AssignmentRequestQueueConfiguration
    {
        public int ChannelSize { get; set; }

        public AssignmentRequestQueueConfiguration()
        {
        }

        public AssignmentRequestQueueConfiguration(int channelSize)
        {
            ChannelSize = channelSize;
        }
    }
}
