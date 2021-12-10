
using System.ComponentModel.DataAnnotations;

namespace blendnet.cms.api.Model
{
    /// <summary>
    /// Request class for updating subscription's end date
    /// </summary>
    public class UpdateSubscriptionEndDateRequest
    {
        /// <summary>
        /// End date to be set
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }
    }
}
