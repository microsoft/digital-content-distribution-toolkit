using blendnet.cms.repository.Interfaces;
using blendnet.common.dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace blendnet.cms.api.Controllers
{
    /// <summary>
    /// Controller for browsing active subscriptions
    /// to be used by clients
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class BrowseSubscriptionController : Controller
    {
        private readonly ILogger _logger;

        private IContentProviderRepository _contentProviderRepository;

        public BrowseSubscriptionController(ILogger<BrowseSubscriptionController> logger, IContentProviderRepository contentProviderRepository)
        {
            _logger = logger;

            _contentProviderRepository = contentProviderRepository;
        }

        /// <summary>
        /// Gets all active subscriptions i.e. those where startDate <= currentTime <= endDate
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        [HttpGet("{contentProviderId:guid}", Name = nameof(GetActiveSubscriptions))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<ContentProviderSubscriptionDto>>> GetActiveSubscriptions(Guid contentProviderId)
        {
            var allSubscriptions = await this._contentProviderRepository.GetSubscriptions(contentProviderId);
            var now = DateTime.UtcNow;
            var activeSubscriptions = (from o in allSubscriptions
                                       where o.StartDate <= now
                                       where o.EndDate >= now
                                       orderby o.CreatedDate descending
                                       select o).ToList();
            return activeSubscriptions;
        }
    }
}
