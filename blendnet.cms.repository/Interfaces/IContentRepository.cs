using blendnet.common.dto;
using blendnet.common.dto.Cms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.repository.Interfaces
{
    public interface IContentRepository
    {
        /// <summary>
        /// List all contents
        /// </summary>
        /// <returns></returns>
        Task<List<Content>> GetContents();

        /// <summary>
        /// Get Content  by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Content> GetContentById(Guid id);

        /// <summary>
        /// Upload Content 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        Task<bool> UploadContent(List<Content> contents);

        /// <summary>
        /// Update Content 
        /// </summary>
        /// <param name="updatedContent"></param>
        /// <returns></returns>
        Task<int> UpdateContent(Content updatedContent);

        /// <summary>
        /// Delete Content 
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        Task<int> DeleteContent(Guid contentId);
    }
}