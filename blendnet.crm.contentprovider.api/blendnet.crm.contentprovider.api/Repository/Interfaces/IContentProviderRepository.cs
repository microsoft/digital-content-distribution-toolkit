//using blendnet.crm.common.dto;
//using blendnet.crm.contentprovider.api.Model;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace blendnet.crm.contentprovider.api.Repository.Interfaces
//{
//    /// <summary>
//    /// Content Provider and Content Admistrator Repository
//    /// </summary>
//    public interface IContentProviderRepository
//    {
//        /// <summary>
//        /// List all content providers
//        /// </summary>
//        /// <returns></returns>
//        Task<List<ContentProviderDto>> GetContentProviders();

//        /// <summary>
//        /// Get Content Provider by Id
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        Task<ContentProviderDto> GetContentProviderById(Guid id);

//        /// <summary>
//        /// Creates Content Provider
//        /// </summary>
//        /// <param name="contentProvider"></param>
//        /// <returns></returns>
//        Task<Guid> CreateContentProvider(ContentProviderDto contentProvider);
        
//        /// <summary>
//        /// Update Content Provider
//        /// </summary>
//        /// <param name="updatedContentProvider"></param>
//        /// <returns></returns>
//        Task<int> UpdateContentProvider(ContentProviderDto updatedContentProvider);

//        /// <summary>
//        /// Delete Content Provider
//        /// </summary>
//        /// <param name="contentProviderId"></param>
//        /// <returns></returns>
//        Task<int> DeleteContentProvider(Guid contentProviderId);
//    }
//}
