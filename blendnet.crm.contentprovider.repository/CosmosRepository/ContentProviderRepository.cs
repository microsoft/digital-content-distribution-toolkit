using blendnet.common.dto;
using blendnet.crm.contentprovider.repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.crm.contentprovider.repository.CosmosRepository
{
    public class ContentProviderRepository : IContentProviderRepository
    {
        private BlendNetContext _blendNetContext;

        private readonly ILogger _logger;

        public ContentProviderRepository(BlendNetContext blendNetContext, ILogger<ContentProviderRepository> logger)
        {
            _blendNetContext = blendNetContext;

            _logger = logger;
        }

        #region Content Provider Methods
        /// <summary>
        /// Creates the content provider
        /// </summary>
        /// <param name="contentProvider"></param>
        /// <returns></returns>
        public async Task<Guid> CreateContentProvider(ContentProviderDto contentProvider)
        {
            contentProvider.SetIdentifiers();

            _blendNetContext.ContentProviders.Add(contentProvider);

            await _blendNetContext.SaveChangesAsync();

            return contentProvider.Id.Value;
        }

        /// <summary>
        /// Get the content provider by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ContentProviderDto> GetContentProviderById(Guid id)
        {
            return _blendNetContext.ContentProviders.Where(cp => cp.Id == id).AsNoTracking().FirstOrDefault();
        }

        /// <summary>
        /// Return the content providers
        /// </summary>
        /// <returns></returns>
        public async Task<List<ContentProviderDto>> GetContentProviders()
        {
            return await _blendNetContext.ContentProviders.ToListAsync();
        }

        /// <summary>
        /// Updates Content Provider
        /// </summary>
        /// <param name="updatedContentProvider"></param>
        /// <returns></returns>
        public async Task<int> UpdateContentProvider(ContentProviderDto updatedContentProvider)
        {
            int recordsAffected = 0;

            ContentProviderDto existingProvider = await GetContentProviderById(updatedContentProvider.Id.Value);

            if (existingProvider != default(ContentProviderDto))
            {
                var updatedContentProviderEntry = _blendNetContext.Add(updatedContentProvider);

                updatedContentProviderEntry.State = EntityState.Unchanged;

                _blendNetContext.ContentProviders.Update(updatedContentProvider);

                recordsAffected = await _blendNetContext.SaveChangesAsync();
            }

            return recordsAffected;
        }

        /// <summary>
        /// Deletes Content Provider
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        public async Task<int> DeleteContentProvider(Guid contentProviderId)
        {
            int recordsAffected = 0;

            ContentProviderDto existingProvider = await GetContentProviderById(contentProviderId);

            if (existingProvider != default(ContentProviderDto))
            {
                var deleteContentProviderEntry = _blendNetContext.Add(existingProvider);

                deleteContentProviderEntry.State = EntityState.Unchanged;

                _blendNetContext.ContentProviders.Remove(existingProvider);

                recordsAffected = await _blendNetContext.SaveChangesAsync();
            }

            return recordsAffected;
        }

        #endregion
    }
}
