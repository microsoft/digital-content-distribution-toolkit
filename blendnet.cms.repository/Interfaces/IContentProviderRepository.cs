using blendnet.common.dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.repository.Interfaces
{
    public interface IContentProviderRepository
    {
        /// <summary>
        /// List all content providers
        /// </summary>
        /// <returns></returns>
        Task<List<ContentProviderDto>> GetContentProviders();

        /// <summary>
        /// Get Content Provider by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ContentProviderDto> GetContentProviderById(Guid id);

        /// <summary>
        /// Creates Content Provider
        /// </summary>
        /// <param name="contentProvider"></param>
        /// <returns></returns>
        Task<Guid> CreateContentProvider(ContentProviderDto contentProvider);

        /// <summary>
        /// Update Content Provider
        /// </summary>
        /// <param name="updatedContentProvider"></param>
        /// <returns></returns>
        Task<int> UpdateContentProvider(ContentProviderDto updatedContentProvider);

        /// <summary>
        /// Delete Content Provider
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        Task<int> DeleteContentProvider(Guid contentProviderId);
    }
}
