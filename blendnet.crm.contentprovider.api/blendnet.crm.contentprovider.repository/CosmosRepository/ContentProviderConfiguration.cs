using blendnet.crm.common.dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace blendnet.crm.contentprovider.repository.CosmosRepository
{
    public class ContentProviderConfiguration : IEntityTypeConfiguration<ContentProviderDto>
    {
        public void Configure(EntityTypeBuilder<ContentProviderDto> builder)
        {
            builder.ToContainer("ContentProvider");

            builder.Property(e => e.Id).HasConversion<string>();

            builder.HasKey(cp => cp.Id);

            builder.HasPartitionKey(cp => cp.Id);

            builder.OwnsOne(cp => cp.Address).OwnsOne(cp => cp.MapLocation);

            builder.OwnsMany(cp => cp.ContentAdministrators).OwnsOne(ca => ca.Address).OwnsOne(ca => ca.MapLocation); ;
        }
    }
}
