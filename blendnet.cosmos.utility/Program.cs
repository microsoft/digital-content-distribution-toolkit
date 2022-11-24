// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.cms.repository.CosmosRepository;
using blendnet.cms.repository.Interfaces;
using blendnet.common.dto.cms;
using blendnet.cosmos.utility.BroadCastMigration;
using blendnet.cosmos.utility.Repository;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using blendnet.device.repository.Interfaces;
using blendnet.device.repository.CosmosRepository;
using blendnet.cosmos.utility.DeviceFilterMigration;
using blendnet.common.dto.Device;

namespace blendnet.cosmos.utility
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
             .ConfigureLogging(logging =>
             {
                 logging.ClearProviders();

                 logging.AddConsole();

                 logging.AddDebug();

             })
             .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    services.AddTransient<GenericRepository>();

                    services.AddTransient<IContentRepository, ContentRepository>();

                    services.AddTransient<IDeviceRepository, DeviceRepository>();

                    services.AddTransient<BroadcastMigrationWorker>();

                    services.AddTransient<DeviceFilterMigrationWorker>();

                    //Configure the Cosmos DB
                    ConfigureCosmosDB(hostContext, services);

                    // Configure mapper
                    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

                    //cms app settings
                    services.Configure<AppSettings>(hostContext.Configuration);

                    //device app settings
                    services.Configure<DeviceAppSettings>(hostContext.Configuration);

                });

        private static void ConfigureCosmosDB(HostBuilderContext context, IServiceCollection services)
        {
            string account = context.Configuration.GetValue<string>("AccountEndPoint");

            string databaseName = context.Configuration.GetValue<string>("DatabaseName");

            string key = context.Configuration.GetValue<string>("AccountKey");

            services.AddSingleton<CosmosClient>((cc) => {

                CosmosClient client = new CosmosClientBuilder(account, key)
                           .WithSerializerOptions(new CosmosSerializationOptions()
                           {
                               PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                           })
                           .Build();

                return client;
            });
        }
    }
}
