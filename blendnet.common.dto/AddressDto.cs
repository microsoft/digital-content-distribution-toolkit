using System.ComponentModel.DataAnnotations;

namespace blendnet.common.dto
{
    public class AddressDto
    {
        /// <summary>
        /// Address Line 1
        /// </summary>
        [Required]

        public string Address1 { get; set; }

        /// <summary>
        /// Address Line 2
        /// </summary>
        public string Address2 { get; set; }

        /// <summary>
        /// Address Line 3
        /// </summary>
        public string Address3 { get; set; }
        
        /// <summary>
        /// City
        /// </summary>
        [Required]
        public string City { get; set; }
        
        /// <summary>
        /// State
        /// </summary>
        [Required]
        public string State { get; set; }
        
        /// <summary>
        /// PinCode
        /// </summary>
        [Required]
        public int PinCode { get; set; }

        /// <summary>
        /// Location data
        /// </summary>
        [Required]
        public MapLocationDto MapLocation { get; set; }
    }
}