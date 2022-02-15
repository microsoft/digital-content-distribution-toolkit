using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using blendnet.api.proxy.KaizalaIdentity;
using blendnet.common.dto;
using blendnet.common.dto.Incentive;
using blendnet.common.infrastructure;
using blendnet.common.infrastructure.ApplicationInsights;
using blendnet.common.infrastructure.KeyVault;
using blendnet.common.infrastructure.ServiceBus;
using blendnet.incentive.listener.IntegrationEventHandling;
using blendnet.incentive.repository.IncentiveRepository;
using blendnet.incentive.repository.Interfaces;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace blendnet.incentive.listener
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            Log.Logger = new LoggerConfiguration()
           .ReadFrom.Configuration(configuration)
           .Enrich.FromLogContext()
           .CreateLogger();

            CreateHostBuilder(args).Build().Run();

            Log.CloseAndFlush();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                 .ConfigureLogging(logging =>
                 {
                     logging.ClearProviders();

                     logging.AddConsole();

                     logging.AddDebug();

                     logging.AddSerilog();
                 })
                .ConfigureAppConfiguration((context, config) =>
                {
                    //read the configuration from keyvault in case of production
                    //https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-5.0
                    if (context.HostingEnvironment.IsProduction())
                    {
                        var builtConfig = config.Build();

                        var secretClient = new SecretClient(
                        new Uri($"https://{builtConfig["KeyVaultName"]}.vault.azure.net/"),
                        new DefaultAzureCredential());

                        config.AddAzureKeyVault(secretClient, new PrefixKeyVaultSecretManager(builtConfig["KeyVaultPrefix"]));
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    //Configure Application Settings
                    services.Configure<IncentiveAppSettings>(hostContext.Configuration);

                    services.AddLogging();

                    services.AddHostedService<EventListener>();

                    //set up application insights
                    services.AddSingleton<ITelemetryInitializer, BlendNetTelemetryInitializer>();
                    services.AddApplicationInsightsTelemetryWorkerService();

                    string serviceBusConnectionString = hostContext.Configuration.GetValue<string>("ServiceBusConnectionString");

                    string userDataStorageConnectionString = hostContext.Configuration.GetValue<string>("UserDataStorageConnectionString");

                    services.AddAzureClients(builder =>
                    {
                        // Register blob service client and initialize it using the Storage section of configuration
                        builder.AddBlobServiceClient(userDataStorageConnectionString)
                                .WithName(ApplicationConstants.StorageInstanceNames.UserDataStorage)
                                .WithVersion(BlobClientOptions.ServiceVersion.V2019_02_02);

                        //Add Service Bus Client
                        builder.AddServiceBusClient(serviceBusConnectionString);

                    });

                    //Configure Event 
                    ConfigureEventBus(hostContext, services);

                    //Configure the Cosmos DB
                    ConfigureCosmosDB(hostContext, services);

                    //Configure Http Clients
                    ConfigureHttpClients(services);

                    //Configure Distribute Cache
                    ConfigureDistributedCache(hostContext, services);

                    //Configure Repository
                    services.AddTransient<IIncentiveRepository, IncentiveRepository>();

                    services.AddTransient<IEventRepository, EventRepository>();

                    //Configure Kaizala Identity Proxy
                    services.AddTransient<KaizalaIdentityProxy>();
                });


        /// <summary>
        /// Configure Event Bus
        /// </summary>
        /// <param name="services"></param>
        private static void ConfigureEventBus(HostBuilderContext context, IServiceCollection services)
        {
            //event bus related registrations
            string serviceBusConnectionString = context.Configuration.GetValue<string>("ServiceBusConnectionString");

            string serviceBusTopicName = context.Configuration.GetValue<string>("ServiceBusTopicName");

            string serviceBusSubscriptionName = context.Configuration.GetValue<string>("ServiceBusSubscriptionName");

            int serviceBusMaxConcurrentCalls = context.Configuration.GetValue<int>("ServiceBusMaxConcurrentCalls");

            services.AddSingleton<EventBusConnectionData>(ebcd =>
            {
                EventBusConnectionData eventBusConnectionData = new EventBusConnectionData();

                eventBusConnectionData.ServiceBusConnectionString = serviceBusConnectionString;

                eventBusConnectionData.TopicName = serviceBusTopicName;

                eventBusConnectionData.SubscriptionName = serviceBusSubscriptionName;

                eventBusConnectionData.MaxConcurrentCalls = serviceBusMaxConcurrentCalls;

                return eventBusConnectionData;
            });

            services.AddSingleton<IEventBus, EventServiceBus>();

            services.AddTransient<OrderCompletedEventIntegrationEventHandler>();
            services.AddTransient<RetailerAssignedIntegrationEventHandler>();
            services.AddTransient<UserIntegrationEventHandler>();
            services.AddTransient<UserSigninIncentiveIntegrationEventHandler>();
            services.AddTransient<UserAppOpenIncentiveIntegrationEventHandler>();
            services.AddTransient<UserStreamContentIncentiveIntegrationEventHandler>();
            services.AddTransient<UserOnboardingRatingSubmittedIncentiveIntegrationEventHandler>();
            services.AddTransient<ExportUserDataIntegrationEventHandler>();
            services.AddTransient<UpdateUserDataIntegrationEventHandler>();

        }

        /// <summary>
        /// Set up Cosmos DB
        /// </summary>
        /// <param name="context"></param>
        /// <param name="services"></param>
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

                DatabaseResponse database = client.CreateDatabaseIfNotExistsAsync(databaseName).Result;

                ContainerResponse containerResponse = database.Database.CreateContainerIfNotExistsAsync(ApplicationConstants.CosmosContainers.IncentivePlan, ApplicationConstants.CosmosContainers.IncentivePlanPartitionKey).Result;

                containerResponse = database.Database.CreateContainerIfNotExistsAsync(ApplicationConstants.CosmosContainers.IncentiveEvent, ApplicationConstants.CosmosContainers.IncentiveEventPartitionKey).Result;

                return client;
            });
        }

        /// <summary>
        /// Configure Required Http Clients
        /// </summary>
        /// <param name="services"></param>
        private static void ConfigureHttpClients(IServiceCollection services)
        {
            //Configure Http Clients
            services.AddHttpClient(ApplicationConstants.HttpClientKeys.KAIZALA_HTTP_CLIENT, c =>
            {
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });
        }

        /// <summary>
        /// Configures Redis as distributed cache
        /// </summary>
        /// <param name="services"></param>
        private static void ConfigureDistributedCache(HostBuilderContext hostContext, IServiceCollection services)
        {
            string redisCacheConnectionString = hostContext.Configuration.GetValue<string>("RedisCacheConnectionString");

            services.AddStackExchangeRedisCache(options => {
                options.Configuration = redisCacheConnectionString;
            });

        }
    }
}
