using System.ComponentModel.DataAnnotations;

namespace blendnet.device.api.Model
{
    public class GetDeviceContentRequest
    {
        /// <summary>
        /// Continuation Token
        /// </summary>
        public string ContinuationToken { get; set; }

        /// <summary>
        /// page size
        /// </summary>
        [Range(1, 200)]
        public int PageSize { get; set; } = 100;
    }
}
