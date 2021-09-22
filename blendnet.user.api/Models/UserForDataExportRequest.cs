namespace blendnet.user.api.Models
{
    /// <summary>
    /// Request class for Get Data Export Request for user
    /// </summary>
    public class UserForDataExportRequest
    {
        // <summary>
        /// Phone Number of user. Needed only when called by admin
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}