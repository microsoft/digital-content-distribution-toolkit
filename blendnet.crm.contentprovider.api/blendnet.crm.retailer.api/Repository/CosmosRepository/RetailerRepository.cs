using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blendnet.crm.common.dto;
using blendnet.crm.retailer.api.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace blendnet.crm.retailer.api.Repository.CosmosRepository
{
    public class RetailerRepository : IRetailerRepository
    {
        private BlendNetContext _blendNetContext;

        private readonly ILogger _logger;
        
        public RetailerRepository(BlendNetContext blendNetContext, ILogger<RetailerRepository> logger)
        {
            _blendNetContext = blendNetContext;

            _logger = logger;
        }

        public async Task<Guid> CreateRetailer(RetailerDto retailer)
        {
            retailer.ResetIdentifiers();

            _blendNetContext.Retailers.Add(retailer);

            await _blendNetContext.SaveChangesAsync();

            return retailer.Id.Value;
        }

        public async Task<int> DeleteRetailer(Guid retailerId)
        {
            int recordsAffected = 0;

            RetailerDto existingRetailer = await GetRetailerById(retailerId);

            if (existingRetailer != default(RetailerDto))
            {
                var deleteRetailerEntry = _blendNetContext.Add(existingRetailer);

                deleteRetailerEntry.State = EntityState.Unchanged;

                _blendNetContext.Retailers.Remove(existingRetailer);

                recordsAffected = await _blendNetContext.SaveChangesAsync();
            }

            return recordsAffected;
        }

        public async Task<RetailerDto> GetRetailerById(Guid id)
        {
            return _blendNetContext.Retailers.Where(r => r.Id == id).AsNoTracking().FirstOrDefault();
        }

        public async Task<List<RetailerDto>> GetRetailers()
        {
            return await _blendNetContext.Retailers.ToListAsync();
        }

        public async Task<int> UpdateRetailer(RetailerDto updatedRetailer)
        {
            int recordsAffected = 0;

            RetailerDto existingRetailer = await GetRetailerById(updatedRetailer.Id.Value);

            if (existingRetailer != default(RetailerDto))
            {
                var updatedRetailerEntry = _blendNetContext.Add(updatedRetailer);

                updatedRetailerEntry.State = EntityState.Unchanged;

                _blendNetContext.Retailers.Update(updatedRetailer);

                recordsAffected = await _blendNetContext.SaveChangesAsync();
            }
            
            return recordsAffected;
        }
    }
}