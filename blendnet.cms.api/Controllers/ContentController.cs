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
        /// List all contents 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<Content>>> GetContents()
        {
            var content = await _contentRepository.GetContents();

            return Ok(content);
        }

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
        [HttpPost]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult<string>> UploadContent(IFormFile file)
        {
            StreamReader reader = new StreamReader(file.OpenReadStream());

            string text = reader.ReadToEnd();

            List<Content> contents = JsonConvert.DeserializeObject<List<Content>>(text);

            var contentId = await _contentRepository.UploadContent(contents);

            // //publish the event
            // ContentProviderCreatedIntegrationEvent contentProviderCreatedIntegrationEvent = new ContentProviderCreatedIntegrationEvent()
            // {
            //     Content = content,
            // };
            // await _eventBus.Publish(contentProviderCreatedIntegrationEvent);

            return Ok(contentId);

        }

        #endregion

    }
}