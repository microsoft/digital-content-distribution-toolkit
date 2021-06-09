using blendnet.common.dto;
using blendnet.common.dto.Retailer;
using blendnet.retailer.repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
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

        IStringLocalizer<SharedResource> _stringLocalizer;

        private const double DISTANCE_METERS_MIN = 500; // 500m
        private const double DISTANCE_METERS_MAX = 100 * 1000; // 100km

        public BrowseRetailerController(ILogger<BrowseRetailerController> logger, IRetailerRepository retailerRepository, IStringLocalizer<SharedResource> stringLocalizer)
        {
            this._logger = logger;
            this._retailerRepository = retailerRepository;
            _stringLocalizer = stringLocalizer;
        }

        [HttpGet("nearby")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<RetailerWithDistanceDto>>> GetRetailersForLocation( double lat,
                                                                                                double lng,
                                                                                                double distanceMeters)
        {
            // normalize distance within range
            distanceMeters = distanceMeters >= DISTANCE_METERS_MIN ? distanceMeters : DISTANCE_METERS_MIN;
            distanceMeters = distanceMeters <= DISTANCE_METERS_MAX ? distanceMeters : DISTANCE_METERS_MAX;

            List<string> errors = new List<string>();

            var requestedLocation = new MapLocationDto()
            {
                Latitude = lat,
                Longitude = lng,
            };

            if (!requestedLocation.isValid())
            {
                errors.Add(_stringLocalizer["RMS_ERR_0005"]);
            }

            if (errors.Count > 0)
            {
                return BadRequest(errors);
            }

            List<RetailerWithDistanceDto> nearbyActiveRetailers = await this._retailerRepository.GetNearbyRetailers(lat, lng, distanceMeters, false /* shouldGetInactive*/);

            return nearbyActiveRetailers;
        }
    }
}
