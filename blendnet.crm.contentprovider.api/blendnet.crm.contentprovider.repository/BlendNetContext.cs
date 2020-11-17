using blendnet.crm.common.dto;
using blendnet.crm.contentprovider.repository.CosmosRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace blendnet.crm.contentprovider.repository
{
    public class BlendNetContext : DbContext
    {
        public BlendNetContext(DbContextOptions<BlendNetContext> options)
           : base(options)
        {

        }

        /// <summary>
        /// https://github.com/dotnet/efcore/issues/21763
        /// https://github.com/dotnet/efcore/issues/17751
        /// https://github.com/dotnet/efcore/issues/17751
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultContainer("ContentProvider");

            // modelBuilder.ApplyConfiguration(new RetailerConfiguration());

            modelBuilder.ApplyConfiguration(new ContentProviderConfiguration());
        }

        // public DbSet<RetailerDto> Retailers { get; set; }

        public DbSet<ContentProviderDto> ContentProviders { get; set; }
    }
}
