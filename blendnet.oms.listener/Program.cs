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

                services.AddAzureClients(builder =>
                {
                    //Add Service Bus Client
                    builder.AddServiceBusClient(serviceBusConnectionString);

                });

                //Configure Event 
                ConfigureEventBus(hostContext, services);

                //Configure Http Clients
                ConfigureHttpClients(hostContext, services);

                //Configure Distribute Cache
                ConfigureDistributedCache(hostContext, services);

                //Configure User Proxy
                services.AddTransient<UserProxy>();

                //Configure Kaizala Notification Proxy
                services.AddTransient<NotificationProxy>();
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

#endregion





