﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using blendnet.common.infrastructure;
using blendnet.common.dto;
using blendnet.common.dto.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using blendnet.crm.user.api.Repository.Interfaces;
using blendnet.common.dto.Events;
using Microsoft.Extensions.Options;
using blendnet.crm.user.api.Model;
using Microsoft.AspNetCore.Authorization;

namespace blendnet.crm.user.api.Controllers
{
    /// <summary>
    /// Controller created to support Authorization in Azure AD B2C
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    //[Authorize]
    public class IdentityController : ControllerBase
    {
        private readonly ILogger _logger;

        private IIdentityRespository _identityRepository;

        private IEventBus _eventBus;

        

        public IdentityController(  IIdentityRespository identityRepository, 
                                    ILogger<IdentityController> logger,
                                    IEventBus eventBus)
        {
            _identityRepository = identityRepository;

            _logger = logger;

            _eventBus = eventBus;

        }

        /// <summary>
        /// Returns the list of groups for which the user is direct member of
        /// </summary>
        /// <param name="inputClaims"></param>
        /// <returns></returns>
        [HttpPost("listmemberof")]
        public async Task<ActionResult<OutputClaimsDto>> ListMemberOf(InputClaimsDto inputClaims)
        {
            if (inputClaims == null)
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseDto("Request content is null", HttpStatusCode.Conflict));
            }

            if (string.IsNullOrEmpty(inputClaims.ObjectId))
            {
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseDto("Request object is null or empty", HttpStatusCode.Conflict));
            }

            try
            {
                List<string> groups = await _identityRepository.ListMemberOf(inputClaims.ObjectId);
                                
                OutputClaimsDto output = new OutputClaimsDto() { groups = groups };

                return Ok(output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                if (ex.Message.Contains("Request_ResourceNotFound"))
                {
                    return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseDto("Can not read user groups, user not found", HttpStatusCode.Conflict));
                }
                
                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseDto("Can not read user groups", HttpStatusCode.Conflict));
            }
        }

        /// <summary>
        /// Returns the user
        /// </summary>
        /// <param name="upn"></param>
        /// <returns></returns>
        [HttpGet("getuser")]
        public async Task<ActionResult<IdentityUserDto>> GetUser(string upn)
        {
            IdentityUserDto identityUser = await _identityRepository.GetUser(upn);

            if (identityUser != null)
            {
                return Ok(identityUser);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Gets invoked by Azure B2C on User Sign up
        /// </summary>
        /// <param name="inputClaims"></param>
        /// <returns></returns>
        [HttpPost("createuserprofile")]
        public async Task<ActionResult> CreateUserProfile(InputClaimsDto inputClaims)
        {
            UserChangedIntegrationEvent userChangedIntegrationEvent = new UserChangedIntegrationEvent()
            {
                FirstName = "Hk",
                GivenName = "GK",
                LastName = "LN",
            };

            UserDummyIntegrationEvent userDummyIntegrationEvent = new UserDummyIntegrationEvent()
            {
                
            };

            await _eventBus.Publish(userChangedIntegrationEvent);

            await _eventBus.Publish(userDummyIntegrationEvent);

            return Ok();
        }

        /// <summary>
        /// Gets invoked by Azure B2C on User Profile Edit
        /// </summary>
        /// <param name="inputClaims"></param>
        /// <returns></returns>
        [HttpPost("updateuserprofile")]
        public async Task<ActionResult> UpdateUserProfile(InputClaimsDto inputClaims)
        {
            return null;
        }

        
    }
}
