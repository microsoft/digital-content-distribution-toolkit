// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using AutoMapper;
using blendnet.cms.repository.Interfaces;
using blendnet.common.dto.Cms;
using blendnet.cosmos.utility.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cosmos.utility.BroadCastMigration
{
    /// <summary>
    /// Perform Broadcast Schema Related Changes
    /// </summary>
    public class BroadcastMigrationWorker
    {
        private GenericRepository _genericRepository;

        private IContentRepository _contentRepository;

        private readonly IMapper _mapper;

        private readonly ILogger<BroadcastMigrationWorker> _logger;


        public BroadcastMigrationWorker(IMapper mapper,
                                        ILogger<BroadcastMigrationWorker> logger,
                                        GenericRepository genericRepository,
                                        IContentRepository contentRepository)
        {
            _mapper = mapper;

            _logger = logger;

            _genericRepository = genericRepository;

            _contentRepository = contentRepository;

        }

        /// <summary>
        /// Peform the data migration
        /// </summary>
        /// <returns></returns>
        public async Task DoWork()
        {
            List<OldContent> existingContents = await GetBrodcastingContents();

            ContentCommand contentCommand = null;

            Content newContent = null;

            foreach (OldContent exisingContent in existingContents)
            {
                try
                {
                    contentCommand = await _contentRepository.GetContentCommandById(exisingContent.ContentBroadcastedBy.Value,
                                                         exisingContent.Id.Value);

                    newContent = _mapper.Map<OldContent, Content>(exisingContent);

                    ContentBroadcastedBy contentBroadcastedBy = new ContentBroadcastedBy()
                    {
                        CommandId = contentCommand.Id.Value,
                        BroadcastRequest = contentCommand.BroadcastRequest
                    };

                    newContent.ContentBroadcastedBy = contentBroadcastedBy;
                    newContent.ContentAdvisory = "N/A";
                    newContent.Genre = Genre.Action;
                    newContent.AgeAppropriateness = "13+";

                    await _contentRepository.UpdateContent(newContent);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to update ContentId {exisingContent.Id} Command Id {contentCommand.Id}");
                }
                
            }
        }

        /// <summary>
        /// Get Contents which are in Broadcasted state or Cancelled State
        /// </summary>
        /// <returns></returns>
        private async Task<List<OldContent>> GetBrodcastingContents()
        {
            string query = "select * from c where c.type = 'Content' AND c.contentBroadcastStatus IN ('BroadcastOrderComplete','BroadcastCancelComplete')";

            List<OldContent> existingContents = await _genericRepository.GetList<OldContent>("blendnetdev", "Content", query);

            return existingContents;
        }
    }

}
