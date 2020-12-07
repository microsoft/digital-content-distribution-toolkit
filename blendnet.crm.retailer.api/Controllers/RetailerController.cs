using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blendnet.common.dto;
using blendnet.crm.retailer.repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace blendnet.crm.retailer.api.Controllers
{

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class RetailerController : ControllerBase
    {
        private readonly IRetailerRepository _retailerRepository;
        private readonly ILogger<RetailerController> _logger;

        public RetailerController(IRetailerRepository retailerRepository, ILogger<RetailerController> logger)
        {
            _retailerRepository = retailerRepository;
            _logger = logger;
        }

        #region Retailer Methods

        /// <summary>
        /// List all retailers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<RetailerDto>>> GetRetailers()
        {
            var retailers = await _retailerRepository.GetRetailers();

            return Ok(retailers);
        }

        /// <summary>
        /// Get Retailer
        /// </summary>
        /// <param name="retailerId"></param>
        /// <returns></returns>
        [HttpGet("{retailerId:guid}",Name = nameof(GetRetailer))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<RetailerDto>> GetRetailer(Guid retailerId)
        {
            var retailer = await _retailerRepository.GetRetailerById(retailerId);

            if (retailer != default(RetailerDto))
            {
                return Ok(retailer);
            }else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Create Retailer
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ApiConventionMethod(typeof(DefaultApiConventions),nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult<string>> CreateRetailer(RetailerDto retailer)
        {
            var retailerId = await _retailerRepository.CreateRetailer(retailer);
            
            return CreatedAtAction( nameof(GetRetailer), 
                                    new { retailerId = retailerId, Version = ApiVersion.Default.MajorVersion.ToString() },
                                    retailerId.ToString());
        }

        /// <summary>
        /// Updates the content provider id
        /// </summary>
        /// <param name="retailerId"></param>
        /// <param name="retailer"></param>
        /// <returns></returns>
        [HttpPost("{retailerId:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> UpdateRetailer(Guid retailerId , RetailerDto retailer)
        {
            retailer.Id = retailerId;

            int recordsAffected = await _retailerRepository.UpdateRetailer(retailer);

            if (recordsAffected > 0)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Deletes the retailer
        /// </summary>
        /// <param name="retailerId"></param>
        /// <returns></returns>
        [HttpDelete("{retailerId:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<ActionResult> DeleteRetailer(Guid retailerId)
        {
            int recordsAffected = await _retailerRepository.DeleteRetailer(retailerId);

            if (recordsAffected > 0)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }

        }

        /// <summary>
        /// Activate Retailer
        /// </summary>
        /// <param name="retailerId"></param>
        /// <returns></returns>
        [HttpPost("{retailerId:guid}/activate")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> ActivateRetailer(Guid retailerId)
        {
            return  await ActivateDeactivateRetailer(retailerId, true);
        }

        /// <summary>
        /// Deactivate retailer
        /// </summary>
        /// <param name="retailerId"></param>
        /// <returns></returns>
        [HttpPost("{retailerId:guid}/deactivate")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> DeactivateRetailer(Guid retailerId)
        {
            return await ActivateDeactivateRetailer(retailerId, false);
        }


        /// <summary>
        /// Private method to support activate and deactivate retailer
        /// </summary>
        /// <param name="retailerId"></param>
        /// <param name="activate"></param>
        /// <returns></returns>
        private async Task<ActionResult> ActivateDeactivateRetailer(Guid retailerId, bool activate)
        {
            var retailer = await _retailerRepository.GetRetailerById(retailerId);

            if (retailer != null)
            {
                if (activate)
                {
                    retailer.ActivationDate = DateTime.Now;
                    retailer.IsActive = true;
                }
                else
                {
                    retailer.DeactivationDate = DateTime.Now;
                    retailer.IsActive = false;
                }

                await _retailerRepository.UpdateRetailer(retailer);

                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
    #endregion

    #region Hub Methods
    /// <summary>
    /// List all hubs
    /// </summary>
    /// <returns></returns>
    [HttpGet("{retailerId:guid}/Hubs")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public async Task<ActionResult<List<ContentAdministratorDto>>> GetHubs(Guid retailerId)
    {
        var retailer = await _retailerRepository.GetRetailerById(retailerId);

        if (retailer != default(RetailerDto) 
            && retailer.Hubs != null 
            && retailer.Hubs.Count > 0)
        {
            return Ok(retailer.Hubs);
        }
        else
        {
            return NotFound();
        }
    }

    /// <summary>
    /// This is actually an update on retailer only
    /// </summary>
    /// <param name="retailerId"></param>
    /// <param name="hub"></param>
    /// <returns></returns>
    [HttpPost("{retailerId:guid}/Hubs")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
    public async Task<ActionResult> CreateHub(Guid retailerId,HubDto hub)
    {
        var retailer = await _retailerRepository.GetRetailerById(retailerId);

        if (retailer != null)
        {
            hub.ResetIdentifiers();

            if (retailer.Hubs == null)
            {
                retailer.Hubs = new List<HubDto>();
            }

            retailer.Hubs.Add(hub);

            await _retailerRepository.UpdateRetailer(retailer);

            return NoContent();
        }
        else
        {
            return NotFound();
        }
    }
    

    /// <summary>
    /// Activate Hub
    /// </summary>
    /// <param name="retailerId"></param>
    /// <param name="hubId"></param>
    /// <returns></returns>
    [HttpPost("{retailerId:guid}/Hubs/{hubId:guid}/activate")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
    public async Task<ActionResult> ActivateHub(Guid retailerId, Guid hubId)
    {
        return await ActivateDeactivateHub(retailerId, hubId, true);
    }

    /// <summary>
    /// Deactivate Content Administrator
    /// </summary>
    /// <param name="retailerId"></param>
    /// <param name="hubId"></param>
    /// <returns></returns>
    [HttpPost("{retailerId:guid}/Hubs/{hubId:guid}/deactivate")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
    public async Task<ActionResult> DeactivateHub(Guid retailerId, Guid hubId)
    {
        return await ActivateDeactivateHub(retailerId, hubId, false);
    }

    /// <summary>
    /// Activates or Deactivates hub
    /// </summary>
    /// <param name="retailerId"></param>
    /// <param name="hubId"></param>
    /// <param name="activate"></param>
    /// <returns></returns>
    private async Task<ActionResult> ActivateDeactivateHub(Guid retailerId, Guid hubId, bool activate)
    {
        var retailer = await _retailerRepository.GetRetailerById(retailerId);

        if (retailer != null)
        {
            //Get the existing adminstrator
            var hub = retailer.Hubs.Where(h => h.Id == hubId).FirstOrDefault();

            if (hub == null)
            {
                return NotFound();
            }
            else
            {
                if (activate)
                {
                    hub.ActivationDate = DateTime.Now;

                    hub.IsActive = true;
                }else
                {
                    hub.DeactivationDate = DateTime.Now;

                    hub.IsActive = false;
                }

                await _retailerRepository.UpdateRetailer(retailer);

                return NoContent();
            }
        }
        else
        {
            return NotFound();
        }
    }

    #endregion
    }
}