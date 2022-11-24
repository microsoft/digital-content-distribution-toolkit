// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using blendnet.api.proxy;
using blendnet.api.proxy.Kaizala;
using blendnet.common.dto;
using blendnet.common.dto.Oms;
using blendnet.common.infrastructure;
using blendnet.common.infrastructure.KeyVault;
using blendnet.common.infrastructure.ServiceBus;
using blendnet.oms.listener.IntegrationEventHandling;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using blendnet.api.proxy.Notification;
using Microsoft.ApplicationInsights.Extensibility;
using blendnet.common.infrastructure.ApplicationInsights;
using blendnet.oms.listener;
using Azure.Storage.Blobs;
using blendnet.oms.repository.Interfaces;
using blendnet.oms.repository.CosmosRepository;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;

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
            .ConfigureServices((hostContext, services) =>
            {
                //Configure Application Settings
                services.Configure<OmsAppSettings>(hostContext.Configuration);

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
                ConfigureHttpClients(hostContext, services);

                //Configure Distribute Cache
                ConfigureDistributedCache(hostContext, services);

                //Configure User Proxy
                services.AddTransient<UserProxy>();

                //Configure Kaizala Notification Proxy
                services.AddTransient<NotificationProxy>();

                //Configure Repository
                services.AddTransient<IOMSRepository, OMSRepository>();
            }).
            Build();

await host.RunAsync();

#region Private Methods
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

        services.AddTransient<OrderCompleteEventHandler>();

        services.AddTransient<ExportUserDataIntegrationEventHandler>();

        services.AddTransient<UpdateUserDataIntegrationEventHandler>();

    }

    /// <summary>
    /// Configure Required Http Clients
    /// </summary>
    /// <param name="services"></param>
    static void ConfigureHttpClients(HostBuilderContext hostContext, IServiceCollection services)
    {
        //Configure Http Clients
        services.AddHttpClient(ApplicationConstants.HttpClientKeys.KAIZALA_HTTP_CLIENT, c =>
        {
            c.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        string notificationBaseUrl = hostContext.Configuration.GetValue<string>("NotificationBaseUrl");
        services.AddHttpClient(ApplicationConstants.HttpClientKeys.NOTIFICATION_HTTP_CLIENT, c =>
        {
            c.BaseAddress = new Uri(notificationBaseUrl);
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

        services.AddStackExchangeRedisCache(options => {
            options.Configuration = redisCacheConnectionString;
        });

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

        services.AddSingleton<CosmosClient>((cc) => {

        CosmosClient client = new CosmosClientBuilder(account, key)
                    .WithSerializerOptions(new CosmosSerializationOptions()
                    {
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                    })
                    .Build();

        DatabaseResponse database = client.CreateDatabaseIfNotExistsAsync(databaseName).Result;

        ContainerResponse containerResponse = database.Database.CreateContainerIfNotExistsAsync(ApplicationConstants.CosmosContainers.Order, ApplicationConstants.CosmosContainers.OrderPartitionKey).Result;

        return client;
    });
}

#endregion





