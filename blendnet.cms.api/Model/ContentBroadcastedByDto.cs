namespace blendnet.cms.api.Model
{
    /// <summary>
    /// Broadcast Request
    /// </summary>
    public class ContentBroadcastedByDto
    {
        public BroadcastRequestDto BroadcastRequest { get; set; }
    }

    /// <summary>
    /// Broadcast Info
    /// </summary>
    public class BroadcastRequestDto
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
