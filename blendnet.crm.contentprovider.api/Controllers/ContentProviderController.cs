

namespace blendnet.crm.contentprovider.api{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using blendnet.crm.common.dto;
    //using blendnet.crm.contentprovider
    using Microsoft.AspNetCore.Mvc;
    
    /// <summary>
    /// Content Provider Management
    /// </summary>
    [Route("api/v1/[controller]")]
    public partial class ContentProvidersController:ControllerBase{

        /// <summary>
        /// Get all the Content Providers on BlendNet. This is available only to the platform SA
        /// </summary>
        /// <returns>List of Content Providers</returns>
        /// <response code="201">Returns the list of Content Providers</response>
        /// <response code="500">In the event of any internal error</response> 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContentProviderDto>>> getAllContentProviders(){
             IEnumerable<ContentProviderDto> contentProviders = null;
             return Ok(contentProviders);   
        }

        /// <summary>
        /// Add a new Content Provider
        /// </summary>
        /// <param name="contentProviderDto">ContentProviderDto</param>
        /// <returns>the a ContenProviderDto</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response> 
        [HttpPost]
        public async Task<ActionResult> addNewContentProvider([FromBody]ContentProviderDto contentProviderDto){
            contentProviderDto= new ContentProviderDto();
            //return  CreatedAtRoute("getContentProvider",new ContentProviderDto { id="1"}, contentProviderDto);
            return Ok();
        }

        /// <summary>
        /// get Content Provider details
        /// </summary>
        /// <param name="contentProviderId">Id of the content provider</param>
        /// <returns>ContentProviderDto containing the provider's details</returns>
        /// <response code="400">If the Content Provider with the id is non-existent</response>
        [HttpGet("{contentProviderId}", Name="getContentProvider")]
        public async Task<ActionResult<ContentProviderDto>> getContentProvider([FromRoute] string contentProviderId)
        {
            var contentprovider= new ContentProviderDto(){
                id="test-001"
            };
            return Ok(contentprovider);
        }


        /// <summary>
        /// update the content provider's details
        /// </summary>
        /// <param name="contentProviderId">Id of the content provider</param>
        /// <param name="contentProviderDto">Details of the content provider</param>
        /// <returns>200 on successfully updating the content</returns>
        /// <response code="400">If the Content Provider with the id is non-existent</response>
        [HttpPost("{contentProviderId}")]
        public async Task<ActionResult> updateContentProvider(
            [FromRoute] string contentProviderId,
            [FromBody]ContentProviderDto contentProviderDto){
            return Ok();        
        }

        /// <summary>
        /// Activate an exisiting content provider
        /// </summary>
        /// <param name="contentProviderId">Id of the Content Provider</param>
        /// <returns>200 on successfully updating the Content Provider Details</returns>
        /// <response code="400">If the Content Provider with the id is non-existent</response>
        [HttpPost("{contentProviderId}/activate")]
        public async Task<ActionResult> activateContentProvider([FromRoute]string contentProviderId){
            return Ok();
        }

        /// <summary>
        /// Deactivate an exisiting content provider
        /// </summary>
        /// <param name="contentProviderId">Id of the Content Provider</param>
        /// <returns>200 on successfully updating the Content Provider Details</returns>
        /// <response code="400">If the Content Provider with the id is non-existent</response>
        [HttpPost("{contentProviderId}/deactivate")]
        public async Task<ActionResult> deactivateContentProvider([FromRoute]string contentProviderId){
            return Ok();
        }
    }
}