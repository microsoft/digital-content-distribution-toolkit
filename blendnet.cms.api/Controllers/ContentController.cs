using blendnet.cms.repository.Interfaces;
using blendnet.common.dto;
using blendnet.common.dto.Cms;
using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Net;

namespace blendnet.cms.api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ContentController  : ControllerBase
    {
        private readonly ILogger _logger;

        private IEventBus _eventBus;

        private IContentRepository _contentRepository;

        public ContentController(IContentRepository contentRepository,
                                            ILogger<ContentController> logger,
                                            IEventBus eventBus)
        {
            _contentRepository = contentRepository;

            _logger = logger;

            _eventBus = eventBus;
        }

        #region Content Management Methods


        /// <summary>
        /// Get Content 
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        [HttpGet("{contentId:guid}", Name = nameof(GetContent))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<Content>> GetContent(Guid contentId)
        {
            var content = await _contentRepository.GetContentById(contentId);

            if (content != default(Content))
            {
                return Ok(content);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Upload Contents
        /// </summary>
        /// <returns></returns>
        [HttpPost("{contentProviderId:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult> UploadContent(IFormFile file, Guid contentProviderId)
        {
            string fileExt = System.IO.Path.GetExtension(file.FileName);

            if(fileExt == ".json")
            {
                StreamReader reader = new StreamReader(file.OpenReadStream());
                string text = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
                
                try{
                    List<Content> contents = JsonConvert.DeserializeObject<List<Content>>(text);

                    var contentBool = await _contentRepository.CreateContent(contents,contentProviderId);

                    if(contentBool){
                        //publish the event
                        // foreach(Content content in contents)
                        // {
                        //     ContentUploadedIntegrationEvent contentUploadedIntegrationEvent = new ContentUploadedIntegrationEvent()
                        //     {
                        //        ContentUploadCommand = content,
                        //     };
                        //     await _eventBus.Publish(contentUploadedIntegrationEvent);
                        // }
                        
                        return  Ok(contentBool);
                    }
                    else{
                        return BadRequest();
                    }
                }
                catch(Exception ex){
                    _logger.LogError(ex.Message);
                    return BadRequest();
                }
            }
            else
            {
                string ex = "File not found or not in Json format";
                _logger.LogError(ex);
                return BadRequest();
            }

        }
        #endregion
    }
}

