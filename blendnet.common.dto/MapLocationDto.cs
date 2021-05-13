using System.ComponentModel.DataAnnotations;

namespace blendnet.common.dto
{
    public class MapLocationDto
    {
        /// <summary>
        /// Latitude
        /// </summary>
        [Required]
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        [Required]
        public double Longitude { get; set; }

        /// <summary>
        /// Checks whether this Location has valid co-ordinates
        /// </summary>
        /// <returns>true if Lat and Lng are within range</returns>
        public bool isValid()
        {
            return Latitude >= -90 && Latitude <= 90
                && Longitude >= -180 && Longitude <= 180;
        }
    }
}