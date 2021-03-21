using blendnet.common.dto;
using blendnet.common.dto.Cms;
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
        Task<bool> CreateContent(List<Content> contents,Guid contentProviderId);

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
        Task<List<Content>> GetContentByContentProviderId(Guid contentProviderId);


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
        /// Get Command by Content Id and Command Type
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        Task<List<ContentCommand>> GetCommandByContentId(Guid contentId, common.dto.Cms.CommandType commandType);


    }
}
