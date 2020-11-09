using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using blendnet.crm.common.dto;
using blendnet.crm.common.dto.Identity;
using blendnet.crm.user.api.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace blendnet.crm.user.api.Controllers
{
    /// <summary>
    /// Controller created to support Authorization in Azure AD B2C
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly ILogger _logger;

        private IIdentityRespository _identityRepository;

        public IdentityController(IIdentityRespository identityRepository, ILogger<IdentityController> logger)
        {
            _identityRepository = identityRepository;

            _logger = logger;
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
                if (ex.Message.Contains("Request_ResourceNotFound"))
                {
                    return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseDto("Can not read user groups, user not found", HttpStatusCode.Conflict));
                }

                _logger.LogError(ex, ex.Message);

                return StatusCode((int)HttpStatusCode.Conflict, new B2CResponseDto("Can not read user groups", HttpStatusCode.Conflict));
            }
        }

        [HttpPost("createuserprofile")]
        public async Task<ActionResult> CreateUserProfile(InputClaimsDto inputClaims)
        {
            return null;
        }

        [HttpPost("updateuserprofile")]
        public async Task<ActionResult> UpdateUserProfile(InputClaimsDto inputClaims)
        {
            return null;
        }
    }
}
