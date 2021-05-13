using blendnet.common.dto;
using blendnet.common.dto.Cms;
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
    
        /// <summary>
        /// Generate SaS token
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        Task<SasTokenDto> GenerateSaSToken(Guid contentProviderId);

        /// <summary>
        /// Creates Subscription
        /// </summary>
        /// <param name="subscriptionMetadata">subscription data</param>
        /// <returns>ID of the created subscription</returns>
        Task<Guid> CreateSubscription(ContentProviderSubscriptionDto subscriptionMetadata);

        /// <summary>
        /// Gets all subscriptions for a give content provider
        /// </summary>
        /// <param name="contentProviderId">ID of the content provider</param>
        /// <returns>subscriptions as a list</returns>
        Task<List<ContentProviderSubscriptionDto>> GetSubscriptions(Guid contentProviderId);

        /// <summary>
        /// Gets subscription for given ID and contentProvider ID
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        Task<ContentProviderSubscriptionDto> GetSubscription(Guid contentProviderId, Guid subscriptionId);
        
        /// <summary>
        /// Update a subscription's data
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <param name="subscriptionMetadata"></param>
        /// <returns></returns>
        Task<int> UpdateSubscription(Guid contentProviderId, ContentProviderSubscriptionDto subscriptionMetadata);
        
        /// <summary>
        /// Delete a subscription
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        Task<int> DeleteSubscription(Guid contentProviderId, Guid subscriptionId);
    }
}
