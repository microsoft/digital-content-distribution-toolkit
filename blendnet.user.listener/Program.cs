using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using blendnet.common.infrastructure.KeyVault;
using Serilog;
using Microsoft.Extensions.Azure;
using blendnet.api.proxy.KaizalaIdentity;
using blendnet.common.infrastructure.ServiceBus;
using blendnet.common.infrastructure;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using blendnet.common.dto;
using Microsoft.ApplicationInsights.Extensibility;
using blendnet.common.infrastructure.ApplicationInsights;
using blendnet.user.repository.Interfaces;
using blendnet.user.repository.CosmosRepository;
using blendnet.user.listener.IntegrationEventHandling;
using blendnet.user.listener;
using blendnet.common.dto.User;
using Azure.Storage.Blobs;
using blendnet.user.listener.Common;
using blendnet.api.proxy.Notification;

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
                services.Configure<UserAppSettings>(hostContext.Configuration);

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

                //Configure Repository
                services.AddTransient<IUserRepository, UserRepository>();

                //Configure Kaizala Identity Proxy
                services.AddTransient<KaizalaIdentityProxy>();
                
                services.AddTransient<NotificationProxy>();

                //add user export data event handler
                services.AddTransient<ExportUserDataIntegrationEventHandler>();

                services.AddTransient<DataExportCompletedIntegrationEventHandler>();
                services.AddTransient<IncentiveDataExportCompletedIntegrationEventHandler>();
                services.AddTransient<OrderDataExportCompletedIntegrationEventHandler>();
                services.AddTransient<UserDataExportCompletedIntegrationEventHandler>();

            })
            .Build();

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

        ContainerResponse containerResponse = 
        database.Database.CreateContainerIfNotExistsAsync(ApplicationConstants.CosmosContainers.User, 
                                                          ApplicationConstants.CosmosContainers.UserPartitionKey).Result;

        containerResponse = database.Database.CreateContainerIfNotExistsAsync(ApplicationConstants.CosmosContainers.WhitelistedUser,
                                                                        ApplicationConstants.CosmosContainers.WhitelistedUserPartitionKey)
                                                                        .Result;
        return client;
    });
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

    // Configure Notification Http Client
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
#endregion