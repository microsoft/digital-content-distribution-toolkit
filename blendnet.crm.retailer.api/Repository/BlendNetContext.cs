//using blendnet.common.dto;
//using blendnet.crm.retailer.api.Repository.CosmosRepository;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace blendnet.crm.retailer.api.Repository
//{
//    /// <summary>
//    /// EF CORE Blendnet Context
//    /// </summary>
//    public class BlendNetContext: DbContext
//    {
//        public BlendNetContext(DbContextOptions<BlendNetContext> options)
//           : base(options)
//        {

//        }

//        /// <summary>
//        /// https://github.com/dotnet/efcore/issues/21763
//        /// https://github.com/dotnet/efcore/issues/17751
//        /// https://github.com/dotnet/efcore/issues/17751
//        /// </summary>
//        /// <param name="modelBuilder"></param>
//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            modelBuilder.HasDefaultContainer("Retailer");

//            modelBuilder.ApplyConfiguration(new RetailerConfiguration());
//        }

//        public DbSet<RetailerDto> Retailers { get; set; }

//    }
//}
