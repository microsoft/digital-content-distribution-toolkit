// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using blendnet.cms.listener.IntegrationEventHandling;
using blendnet.common.infrastructure;
using blendnet.common.infrastructure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Azure.Storage.Blobs;
using blendnet.common.infrastructure.KeyVault;
using Microsoft.Extensions.Azure;
using blendnet.common.dto;
using blendnet.cms.repository.Interfaces;
using blendnet.cms.repository.CosmosRepository;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using blendnet.common.dto.cms;
using blendnet.cms.listener.Common;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using blendnet.api.proxy.KaizalaIdentity;
using blendnet.api.proxy.Ses;
using Microsoft.ApplicationInsights.Extensibility;
using blendnet.common.infrastructure.ApplicationInsights;
using blendnet.cms.listener;

var configuration = new ConfigurationBuilder()
.SetBasePath(System.IO.Directory.GetCurrentDirectory())
.AddJsonFile("appsettings.json")
.Build();

Log.Logger = new LoggerConfiguration()
   .ReadFrom.Configuration(configuration)
   .Enrich.FromLogContext()
   .CreateLogger();


IHost host = Host.CreateDefaultBuilder(args)
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
    .ConfigureServices((hostContext,services) =>
    {
        
        //Configure Application Settings
        services.Configure<AppSettings>(hostContext.Configuration);

        services.AddLogging();

        services.AddHostedService<EventListener>();

        //Setup Application Insights
        services.AddSingleton<ITelemetryInitializer, BlendNetTelemetryInitializer>();

        services.AddApplicationInsightsTelemetryWorkerService();

        string cmsStorageConnectionString = hostContext.Configuration.GetValue<string>("CMSStorageConnectionString");

        string cmsCDNStorageConnectionString = hostContext.Configuration.GetValue<string>("CMSCDNStorageConnectionString");

        string broadcastStorageConnectionString = hostContext.Configuration.GetValue<string>("BroadcastStorageConnectionString");

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

            builder.AddBlobServiceClient(broadcastStorageConnectionString)
                    .WithName(ApplicationConstants.StorageInstanceNames.BroadcastStorage)
                    .WithVersion(BlobClientOptions.ServiceVersion.V2019_02_02);

           //Add Service Bus Client
           builder.AddServiceBusClient(serviceBusConnectionString);

        });

        //Configure Event 
        ConfigureEventBus(hostContext, services);

        //Configure the Cosmos DB
        ConfigureCosmosDB(hostContext, services);

        //Configure Http Clients
        ConfigureHttpClients(hostContext, services);

        //Configure Distribute Cache
        ConfigureDistributedCache(hostContext, services);

        //Configure Repository
        services.AddTransient<IContentRepository, ContentRepository>();

        //Configure Segment Downloader
        services.AddTransient<SegmentDowloader>();

        //Configure Tar Generator
        services.AddTransient<TarGenerator>();

        //Configure Kaizala Identity Proxy
        services.AddTransient<KaizalaIdentityProxy>();

        //Configure SES Cancellation Proxy
        services.AddTransient<VODEProxy>();
    })
    .Build();

await host.RunAsync();

#region Private Methods
/// <summary>
/// Http Client Failures
/// </summary>
/// <returns></returns>
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int httpClientRetryCount)
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(httpClientRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

/// <summary>
/// Configure Event Bus
/// </summary>
/// <param name="services"></param>
static void ConfigureEventBus(HostBuilderContext context, IServiceCollection services)
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

    services.AddTransient<ContentProviderUpdatedIntegrationEventHandler>();

    services.AddTransient<ContentUploadedIntegrationEventHandler>();

    services.AddTransient<ContentDeletedIntegrationEventHandler>();

    services.AddTransient<ContentTransformIntegrationEventHandler>();

    services.AddTransient<MediaServiceJobIntegrationEventHandler>();

    services.AddTransient<MicrosoftStorageBlobCreatedIntegrationEventHandler>();

    services.AddTransient<ContentBroadcastIntegrationEventHandler>();

    //Broadcast Partner Event Handler
    services.AddTransient<MomOrderCreatedIntegrationEventHandler>();

    services.AddTransient<MomOrderActiveIntegrationEventHandler>();

    services.AddTransient<MomOrderCompletedIntegrationEventHandler>();

    services.AddTransient<MomOrderCancelledIntegrationEventHandler>();

    services.AddTransient<MomOrderFailedIntegrationEventHandler>();

    services.AddTransient<MomOrderRejectedIntegrationEventHandler>();

    services.AddTransient<MomOrderIntegrationEventHandler>();

    services.AddTransient<ContentBroadcastCancellationIntegrationEventHandler>();

}

/// <summary>
/// Set up Cosmos DB
/// </summary>
/// <param name="context"></param>
/// <param name="services"></param>
static void ConfigureCosmosDB(HostBuilderContext context, IServiceCollection services)
{
    string account = context.Configuration.GetValue<string>("AccountEndPoint");

    string databaseName = context.Configuration.GetValue<string>("DatabaseName");

    string key = context.Configuration.GetValue<string>("AccountKey");

    services.AddSingleton<CosmosClient>((cc) =>
    {

        CosmosClient client = new CosmosClientBuilder(account, key)
                   .WithSerializerOptions(new CosmosSerializationOptions()
                   {
                       PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                   })
                   .Build();

        DatabaseResponse database = client.CreateDatabaseIfNotExistsAsync(databaseName).Result;

        ContainerResponse containerResponse = database.Database.CreateContainerIfNotExistsAsync(ApplicationConstants.CosmosContainers.ContentProvider, ApplicationConstants.CosmosContainers.ContentProviderPartitionKey).Result;

        containerResponse = database.Database.CreateContainerIfNotExistsAsync(ApplicationConstants.CosmosContainers.Content, ApplicationConstants.CosmosContainers.ContentPartitionKey).Result;

        return client;
    });
}

/// <summary>
/// Configure Required Http Clients
/// </summary>
/// <param name="services"></param>
static void ConfigureHttpClients(HostBuilderContext hostContext, IServiceCollection services)
{
    string amsStreamingBaseUrl = hostContext.Configuration.GetValue<string>("AmsStreamingBaseUrl");

    int httpHandlerLifeTimeInMts = hostContext.Configuration.GetValue<int>("HttpHandlerLifeTimeInMts");

    int httpClientRetryCount = hostContext.Configuration.GetValue<int>("HttpClientRetryCount");

    services.AddHttpClient(ApplicationConstants.HttpClientNames.AMS, c =>
    {
        c.BaseAddress = new Uri($"{amsStreamingBaseUrl}");
        c.DefaultRequestHeaders.Add("Accept", "application/json");
    }).SetHandlerLifetime(TimeSpan.FromMinutes(httpHandlerLifeTimeInMts))  //Set lifetime to five minutes
      .AddPolicyHandler(GetRetryPolicy(httpClientRetryCount));

    //Configure Http Clients
    services.AddHttpClient(ApplicationConstants.HttpClientKeys.KAIZALA_HTTP_CLIENT, c =>
    {
        c.DefaultRequestHeaders.Add("Accept", "application/json");
    });


    //Configure Http Client for SES service
    string sesServiceBaseUrl = hostContext.Configuration.GetValue<string>("SESServiceBaseUrl");
    services.AddHttpClient(ApplicationConstants.HttpClientKeys.SESVODE_HTTP_CLIENT, c =>
    {
        c.BaseAddress = new Uri(sesServiceBaseUrl);
        c.DefaultRequestHeaders.Add("Accept", "application/json");
    });
}

/// <summary>
/// Configures Redis as distributed cache
/// </summary>
/// <param name="services"></param>
static void ConfigureDistributedCache(HostBuilderContext hostContext, IServiceCollection services)
{
    string redisCacheConnectionString = hostContext.Configuration.GetValue<string>("RedisCacheConnectionString");

    services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisCacheConnectionString;
    });

}

#endregion