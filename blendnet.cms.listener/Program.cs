using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using blendnet.cms.listener.IntegrationEventHandling;
using blendnet.common.infrastructure;
using blendnet.common.infrastructure.ServiceBus;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using blendnet.cms.listener.Model;
using Azure.Storage.Blobs;
using Serilog.Sinks.File;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using blendnet.common.infrastructure.KeyVault;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Graph.Auth;
using Microsoft.Extensions.Azure;
using blendnet.common.dto;
using blendnet.cms.repository.Interfaces;
using blendnet.cms.repository.CosmosRepository;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using blendnet.common.dto.cms;

namespace blendnet.cms.listener
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

                        var azureServiceTokenProvider = new AzureServiceTokenProvider();

                        var keyVaultClient = new KeyVaultClient(
                            new KeyVaultClient.AuthenticationCallback(
                                azureServiceTokenProvider.KeyVaultTokenCallback));

                        config.AddAzureKeyVault(
                            $"https://{builtConfig["KeyVaultName"]}.vault.azure.net/",
                            keyVaultClient,
                            new PrefixKeyVaultSecretManager(builtConfig["KeyVaultPrefix"]));
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    //Configure Application Settings
                    services.Configure<AppSettings>(hostContext.Configuration);

                    services.AddLogging();

                    services.AddHostedService<EventListener>();

                    services.AddApplicationInsightsTelemetryWorkerService();

                    string cmsStorageConnectionString = hostContext.Configuration.GetValue<string>("CMSStorageConnectionString");

                    string cmsCDNStorageConnectionString = hostContext.Configuration.GetValue<string>("CMSCDNStorageConnectionString");

                    string serviceBusConnectionString = hostContext.Configuration.GetValue<string>("ServiceBusConnectionString");

                    services.AddAzureClients(builder => 
                    {
                        // Register blob service client and initialize it using the Storage section of configuration
                        builder.AddBlobServiceClient(cmsStorageConnectionString)
                                .WithName(ApplicationConstants.StorageInstanceNames.CMSStorage)
                                .WithVersion(BlobClientOptions.ServiceVersion.V2019_02_02);

                        builder.AddBlobServiceClient(cmsCDNStorageConnectionString)
                                .WithName(ApplicationConstants.StorageInstanceNames.CMSCDNStorage)
                                .WithVersion(BlobClientOptions.ServiceVersion.V2019_02_02);

                        //Add Service Bus Client
                        builder.AddServiceBusClient(serviceBusConnectionString);

                    });

                    //Configure Event 
                    ConfigureEventBus(hostContext, services);

                    //Configure Microsoft Graph Client
                    ConfigureGraphClient(hostContext, services);

                    //Configure the Cosmos DB
                    ConfigureCosmosDB(hostContext, services);

                    //Configure Repository
                    services.AddTransient<IContentRepository, ContentRepository>();

                });


        /// <summary>
        /// Configure Microsoft Graph Client
        /// </summary>
        /// <param name="context"></param>
        /// <param name="services"></param>
        private static void ConfigureGraphClient (HostBuilderContext context,IServiceCollection services)
        {
            string graphClientId = context.Configuration.GetValue<string>("GraphClientId");

            string graphClientTenant = context.Configuration.GetValue<string>("GraphClientTenant");

            string graphClientSecret = context.Configuration.GetValue<string>("GraphClientSecret");

            //Register graph authentication provider
            services.AddTransient<IAuthenticationProvider>(iap => {

                IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
               .Create(graphClientId)
               .WithTenantId(graphClientTenant)
               .WithClientSecret(graphClientSecret)
               .Build();

                ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);

                return authProvider;
            });

            //register graph
            services.AddTransient<GraphServiceClient>();
        }

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

            services.AddTransient<ContentProviderCreatedIntegrationEventHandler>();

            services.AddTransient<ContentUploadedIntegrationEventHandler>();
            
            services.AddTransient<MicrosoftStorageBlobCreatedIntegrationEventHandler>();
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

                return client;
            });
        }
    }
}
