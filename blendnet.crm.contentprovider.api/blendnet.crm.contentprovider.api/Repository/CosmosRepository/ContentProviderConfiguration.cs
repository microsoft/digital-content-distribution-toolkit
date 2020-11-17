//using blendnet.crm.common.dto;
//using blendnet.crm.contentprovider.api.Model;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Threading.Tasks;

//namespace blendnet.crm.contentprovider.api.Repository.CosmosRepository
//{
//    /// <summary>
//    /// https://github.com/dotnet/efcore/issues/21396
//    /// https://stackoverflow.com/questions/60689345/error-alternate-key-property-id-is-null-with-update-call-to-cosmos-db-and
//    /// </summary>
//    public class ContentProviderConfiguration : IEntityTypeConfiguration<ContentProviderDto>
//    {
//        public void Configure(EntityTypeBuilder<ContentProviderDto> builder)
//        {
//            builder.ToContainer("ContentProvider");

//            builder.Property(e => e.Id).HasConversion<string>();

//            builder.HasKey(cp => cp.Id);
            
//            builder.HasPartitionKey(cp => cp.Id);

//            builder.OwnsOne(cp => cp.Address).OwnsOne(cp => cp.MapLocation);

//            builder.OwnsMany(cp => cp.ContentAdministrators).OwnsOne(ca => ca.Address).OwnsOne(ca => ca.MapLocation); ;
//        }
//    }
//}
