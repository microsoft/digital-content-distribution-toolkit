using AutoMapper;
using blendnet.cms.repository.CosmosRepository;
using blendnet.cms.repository.Interfaces;
using blendnet.common.dto;
using blendnet.common.dto.Cms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace blendnet.cms.api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class BrowseContentController : ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly ILogger _logger;

        private IContentRepository _contentRepository;

        private IContentProviderRepository _contentProviderRepository;

        IStringLocalizer<SharedResource> _stringLocalizer;

        public BrowseContentController(ILogger<BrowseContentController> logger, IContentRepository contentRepository, 
            IContentProviderRepository contentProviderRepository, IMapper mapper, IStringLocalizer<SharedResource> stringLocalizer)
        {
            _logger = logger;

            _contentRepository = contentRepository;

            _contentProviderRepository = contentProviderRepository;

            _mapper = mapper;

            _stringLocalizer = stringLocalizer;
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
        public async Task<ActionResult<ResultData<ContentDto>>> GetProcessedAssets(Guid contentProviderId, string continuationToken)
        {
            List<string> errorInfo = new List<string>();

            ContentStatusFilter contentStatusFilter = new ContentStatusFilter();
            
            contentStatusFilter.ContentTransformStatuses = new string[] { ContentTransformStatus.TransformComplete.ToString() };

            var contentApiResult = await _contentRepository.GetContentByContentProviderId(contentProviderId, contentStatusFilter, continuationToken);

            ResultData<ContentDto> result = new ResultData<ContentDto>(_mapper.Map<List<Content>, List<ContentDto>>(contentApiResult.Data), contentApiResult.ContinuationToken);

            if (result.Data == null || result.Data.Count == 0)
            {
                errorInfo.Add(_stringLocalizer["CMS_ERR_0017"]);

                return BadRequest(errorInfo);
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
        public async Task<ActionResult<ResultData<Content>>> GetBroadcastedAssets(Guid contentProviderId, string continuationToken)
        {
            List<string> errorInfo = new List<string>();

            ContentStatusFilter contentStatusFilter = new ContentStatusFilter();
            
            contentStatusFilter.ContentTransformStatuses = new string[] { ContentTransformStatus.TransformComplete.ToString() };
            
            contentStatusFilter.ContentBroadcastStatuses = new string[] { ContentBroadcastStatus.BroadcastOrderComplete.ToString() };

            var contentApiResult = await _contentRepository.GetContentByContentProviderId(contentProviderId, contentStatusFilter, continuationToken);

            ResultData<ContentDto> result = new ResultData<ContentDto>(_mapper.Map<List<Content>, List<ContentDto>>(contentApiResult.Data), contentApiResult.ContinuationToken);

            if(result.Data == null || result.Data.Count == 0)
            {
                errorInfo.Add(_stringLocalizer["CMS_ERR_0017"]);

                return BadRequest(errorInfo);
            }

            return Ok(result);
        }

        [HttpGet("contentproviders")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<ContentProviderItem>> GetContentProviders()
        {
            var contentProviders = await _contentProviderRepository.GetContentProviders();

            var contentProviderItems = _mapper.Map<List<ContentProviderDto>, List<ContentProviderItem>>(contentProviders);

            return Ok(contentProviderItems);
        }


        #endregion

    }
}
