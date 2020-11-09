using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using blendnet.crm.common.dto;
using blendnet.crm.retailer.api.Repository.Interfaces;
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

    #endregion
    }
}