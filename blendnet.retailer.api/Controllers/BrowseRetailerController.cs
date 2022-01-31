using AutoMapper;
using blendnet.common.dto;
using blendnet.common.dto.Retailer;
using blendnet.retailer.api.Models;
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

        private readonly IRetailerRepository _retailerRepository;

        private readonly IStringLocalizer<SharedResource> _stringLocalizer;

        private readonly IMapper _mapper;

        private const double DISTANCE_METERS_MIN = 500; // 500m
        private const double DISTANCE_METERS_MAX = 100 * 1000; // 100km

        public BrowseRetailerController(    ILogger<BrowseRetailerController> logger, 
                                            IRetailerRepository retailerRepository, 
                                            IStringLocalizer<SharedResource> stringLocalizer, 
                                            IMapper mapper)
        {
            this._logger = logger;
            this._retailerRepository = retailerRepository;
            _stringLocalizer = stringLocalizer;
            _mapper = mapper;
        }

        [HttpGet("nearby")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<RetailerWithDistanceResponse>>> GetRetailersForLocation( double lat,
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

            // keep only active device information
            foreach (var nearbyRetailer in nearbyActiveRetailers)
            {
                nearbyRetailer.Retailer.DeviceAssignments = nearbyRetailer.Retailer.DeviceAssignments.Where(assignment => assignment.IsActive).ToList();
            }

            var nearbyActiveRetailersMapped = _mapper.Map<List<RetailerWithDistanceResponse>>(nearbyActiveRetailers);

            return nearbyActiveRetailersMapped;
        }
    }
}
