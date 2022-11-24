// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Cms;
using blendnet.common.dto.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.repository.Interfaces
{
    /// <summary>
    /// Content Repository Deals with Content Collection.
    /// The same collection stores the Contant and ContentCommand.
    /// Id is Content Id or Command Id.
    /// Partition key is Content Id.
    /// </summary>
    public interface IContentRepository
    {
       /// <summary>
        /// Create Content
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        Task<Guid> CreateContent(Content content);

        /// <summary>
        /// Delete Content
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        Task<int> DeleteContent(Guid contentId);

        /// <summary>
        /// Get Content by Content Id
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        Task<Content> GetContentById(Guid contentId);

        /// <summary>
        /// Returns the list of content by Ids
        /// </summary>
        /// <param name="contentIds"></param>
        /// <returns></returns>
        Task<List<Content>> GetContentByIds(List<Guid> contentIds);

        /// <summary>
        /// Update Content
        /// </summary>
        /// <param name="updatedContent"></param>
        /// <returns></returns>
        Task<int> UpdateContent(Content updatedContent);

        /// <summary>
        /// Get content by content provider id
        /// To Do : Add Paging
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        Task<List<Content>> GetContentByContentProviderId(Guid contentProviderId, ContentStatusFilter contentStatusFilter, bool activeOnly);

        /// <summary>
        /// Get content by provider id, status filter and continuation token
        /// </summary>
        /// <param name="contentProviderId"> Provider id</param>
        /// <param name="contentStatusFilter">Lists of Upload status, Transform status and broadcast status</param>
        /// <param name="continuationToken">continuation token to query</param>
        /// <returns>Content result which holds list of results and continuation token</returns>
        Task<ResultData<Content>> GetContentByContentProviderId(Guid contentProviderId, ContentStatusFilter contentStatusFilter, string continuationToken, bool activeOnly, int pageSize, bool activeBroadcastOnly = false);


        /// <summary>
        /// Create Content Command. Id is command id, partion key is content id
        /// </summary>
        /// <param name="contentCommand"></param>
        /// <returns></returns>
        Task<Guid> CreateContentCommand(ContentCommand contentCommand);

        /// <summary>
        ///  Update Content Command
        /// </summary>
        /// <param name="updatedContent"></param>
        /// <returns></returns>
        Task<int> UpdateContentCommand(ContentCommand updatedContent);

        /// <summary>
        /// Get content command by command id and content id
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="contentId"></param>
        /// <returns></returns>
        Task<ContentCommand> GetContentCommandById(Guid commandId, Guid contentId);

        /// <summary>
        /// Returns the content command by Id
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns></returns>
        Task<ContentCommand> GetContentCommandById(Guid commandId);

        /// <summary>
        /// Get Command by Content Id and Command Type
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        Task<List<ContentCommand>> GetCommandByContentId(Guid contentId, common.dto.Cms.CommandType commandType);

    }
}
