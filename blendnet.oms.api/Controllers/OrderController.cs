using blendnet.api.proxy.Cms;
using blendnet.common.dto.Cms;
using blendnet.oms.repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.oms.api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger _logger;

        private IOMSRepository _omsRepository;

        private ContentProxy _contentProxy;


        public OrderController( IOMSRepository omsRepository,
                                ILogger<OrderController> logger,
                                ContentProxy contentProxy
                                )
        {
            _omsRepository = omsRepository;

            _logger = logger;

            _contentProxy = contentProxy;
        }

        /// <summary>
        /// Returns the Token to view the content
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        [HttpGet("{phoneNumber}/token/{contentId:guid}", Name = nameof(GetContentToken))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<string>> GetContentToken(string phoneNumber, Guid contentId)
        {
            Content content = await _contentProxy.GetContentById(contentId);

            if (content == null)
            {
                return BadRequest($"No valid details found for givent content id {contentId}");
            }

            if (content.ContentTransformStatus != ContentTransformStatus.TransformComplete)
            {
                return BadRequest($"The content tranform status should be complete. Current status is {content.ContentTransformStatus}");
            }
            
            return "";
        }

    }
}
