using AutoMapper;
using blendnet.cms.repository.CosmosRepository;
using blendnet.cms.repository.Interfaces;
using blendnet.common.dto.Cms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace blendnet.cms.api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class BrowseContentController : ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly ILogger _logger;

        private IContentRepository _contentRepository;

        public BrowseContentController(ILogger<BrowseContentController> logger, IContentRepository contentRepository, IMapper mapper)
        {
            _logger = logger;

            _contentRepository = contentRepository;

            _mapper = mapper;
        }

        #region Browse content methods

        /// <summary>
        /// Returns list of all processed assets that is in transform complete state
        /// </summary>
        /// <param name="contentProviderId">Provider id</param>
        /// <param name="continuationToken">Continuation token if available to fetch next set of pages</param>
        /// <returns></returns>
        [HttpPost("{contentProviderId:guid}/processed")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<ContentApiResult<ContentDto>>> GetProcessedAssets(Guid contentProviderId, string continuationToken)
        {
            ContentStatusFilter contentStatusFilter = new ContentStatusFilter();
            contentStatusFilter.ContentTransformStatuses = new string[] { ContentTransformStatus.TransformComplete.ToString() };

            var contentApiResult = await _contentRepository.GetContentByContentProviderId(contentProviderId, contentStatusFilter, continuationToken);

            ContentApiResult<ContentDto> result = new ContentApiResult<ContentDto>(_mapper.Map<List<Content>, List<ContentDto>>(contentApiResult._data), contentApiResult._continuationToken);

            if (result._data == null || result._data.Count == 0)
            {
                return BadRequest("No data found");
            }

            return Ok(result);
        }

        /// <summary>
        /// Returns list of all broadcasted assets that are in broadcast complete state
        /// </summary>
        /// <param name="contentProviderId">Provider id</param>
        /// <param name="continuationToken">Continuation token if available to fetch next set of pages</param>
        /// <returns></returns>
        [HttpPost("{contentProviderId:guid}/broadcasted")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<ContentApiResult<Content>>> GetBroadcastedAssets(Guid contentProviderId, string continuationToken)
        {
            ContentStatusFilter contentStatusFilter = new ContentStatusFilter();
            contentStatusFilter.ContentTransformStatuses = new string[] { ContentTransformStatus.TransformComplete.ToString() };
            contentStatusFilter.ContentBroadcastStatuses = new string[] { ContentBroadcastStatus.BroadcastComplete.ToString() };

            var contentApiResult = await _contentRepository.GetContentByContentProviderId(contentProviderId, contentStatusFilter, continuationToken);

            ContentApiResult<ContentDto> result = new ContentApiResult<ContentDto>(_mapper.Map<List<Content>, List<ContentDto>>(contentApiResult._data), contentApiResult._continuationToken);

            if(result._data == null || result._data.Count == 0)
            {
                return BadRequest("No data found");
            }

            return Ok(result);
        }

        #endregion

    }
}
