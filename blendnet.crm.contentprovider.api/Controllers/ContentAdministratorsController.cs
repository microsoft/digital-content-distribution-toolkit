using System.Collections.Generic;
using System.Threading.Tasks;
using blendnet.crm.common.dto;
using Microsoft.AspNetCore.Mvc;

namespace blendnet.crm.contentprovider.api{
    [Route("api/v1/ContentProviders/{contentProviderId}/[controller]")]
    public partial class ContentAdministratorsController:ControllerBase{
        
        /// <summary>
        /// Get All Content Adminstrators for a Content Provider
        /// </summary>
        /// <param name="contentProviderId">The id of the Content Provider</param>
        /// <returns>List of all Content Administrators</returns>
        /// <response code="201">Returns the list of Content Administrators</response>
        /// <response code="400">Incorrect/Inactive Content Provider </response> 
        /// <response code="500">In the event of any internal error</response> 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContentAdministratorDto>>> getAllContentAdministrators([FromRoute]string contentProviderId){
             IEnumerable<ContentAdministratorDto> contentAdmins = null;
             return Ok(contentAdmins);   
        
        }

        /// <summary>
        /// Add a new Content Administrator to a Content Provider 
        /// </summary>
        /// <param name="contentAdministratorDto">Details of the Content Administrator</param>
        /// <returns>200</returns>
        [HttpPost]
        public async Task<ActionResult> addContentAdministrator
            ([FromBody]ContentAdministratorDto contentAdministratorDto,
            [FromRoute]string contentProviderId){
            
            return Ok();
        }

        /// <summary>
        /// Update the Content Administrator details
        /// </summary>
        /// <param name="contentAdministratorDto"></param>
        /// <returns></returns>
        [HttpPost("{contentAdministratorId}")]
        public async Task<ActionResult> updateContentAdministrator
        ([FromBody]ContentAdministratorDto contentAdministratorDto,
         [FromRoute]string contentProviderId,
         [FromRoute] string contentAdministratorId){
            return Ok();
        }

        [HttpPost("{contentAdministratorId}/activate")]
        public async Task<ActionResult> activateContentAdministrator
        ([FromRoute]string contentProviderId,
         [FromRoute] string contentAdministratorId){
            return Ok();
        }
        
        [HttpPost("{contentAdministratorId}/deactivate")]
        public async Task<ActionResult> deactivateContentAdministrator
        ([FromRoute]string contentProviderId,
         [FromRoute] string contentAdministratorId){
            return Ok();
        }
    }
}