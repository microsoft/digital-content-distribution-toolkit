//using blendnet.common.dto;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace blendnet.crm.retailer.api.Repository.CosmosRepository
//{
//    public class RetailerConfiguration : IEntityTypeConfiguration<RetailerDto>
//    {
//        public void Configure(EntityTypeBuilder<RetailerDto> modelBuilder)
//        {
//            modelBuilder.ToContainer("Retailer");

//            modelBuilder.Property(r => r.Id).HasConversion<string>();

//            modelBuilder.HasKey(r => r.Id);

//            modelBuilder.HasPartitionKey(r => r.Id);

//            modelBuilder.OwnsOne(r => r.Address).OwnsOne(r => r.MapLocation);

//            modelBuilder.OwnsMany(r => r.Hubs).OwnsOne(h => h.Address).OwnsOne(h => h.MapLocation); ;
//        }
//    }
//}
