using blendnet.crm.contentprovider.api.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.crm.contentprovider.api.Repository.CosmosRepository
{
    public class RetailerConfiguration : IEntityTypeConfiguration<Retailer>
    {
        public void Configure(EntityTypeBuilder<Retailer> modelBuilder)
        {
            modelBuilder.ToContainer("Retailer");

            modelBuilder.Property(r => r.Id).HasConversion<string>();

            modelBuilder.HasPartitionKey(r => r.Id);

            modelBuilder.OwnsOne(r => r.Address).OwnsOne(a => a.MapLocation);
        }
    }
}
