using blendnet.common.dto.Retailer;
using blendnet.retailer.repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.retailer.api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class BrowseRetailerController : ControllerBase
    {
        private readonly ILogger _logger;

        private IRetailerRepository _retailerRepository;

        private const double DISTANCE_METERS_MIN = 500; // 100KM
        private const double DISTANCE_METERS_MAX = 100 * 1000; // 100KM

        public BrowseRetailerController(ILogger<BrowseRetailerController> logger, IRetailerRepository retailerRepository)
        {
            this._logger = logger;
            this._retailerRepository = retailerRepository;

        }
        [HttpGet("nearby")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<RetailerWithDistanceDto>>> GetRetailersForLocation( double lat,
                                                                                                double lng,
                                                                                                double distanceMeters)
        {
            distanceMeters = distanceMeters >= DISTANCE_METERS_MIN ? distanceMeters : DISTANCE_METERS_MIN;
            distanceMeters = distanceMeters <= DISTANCE_METERS_MAX ? distanceMeters : DISTANCE_METERS_MAX;

            List<RetailerWithDistanceDto> nearbyActiveRetailers = await this._retailerRepository.GetNearbyRetailers(lat, lng, distanceMeters, false /* shouldGetInactive*/);

            return nearbyActiveRetailers;
        }

    }
}
