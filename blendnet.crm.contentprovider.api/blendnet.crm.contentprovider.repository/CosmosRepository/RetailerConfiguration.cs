using blendnet.crm.common.dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace blendnet.crm.contentprovider.repository.CosmosRepository
{
    public class RetailerConfiguration : IEntityTypeConfiguration<RetailerDto>
    {
        public void Configure(EntityTypeBuilder<RetailerDto> modelBuilder)
        {
            modelBuilder.ToContainer("Retailer");

            modelBuilder.Property(r => r.Id).HasConversion<string>();

            modelBuilder.HasPartitionKey(r => r.Id);

            modelBuilder.OwnsOne(r => r.Address).OwnsOne(a => a.MapLocation);
        }
    }
}
