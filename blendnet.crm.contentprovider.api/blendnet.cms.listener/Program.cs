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

namespace blendnet.cms.listener
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
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
                .ConfigureServices((hostContext, services) =>
                {
                    //Configure Application Settings
                    services.Configure<AppSettings>(hostContext.Configuration);

                    services.AddLogging();

                    services.AddHostedService<EventListener>();

                    services.AddApplicationInsightsTelemetryWorkerService();

                    ConfigureEventBus(hostContext, services);
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

            services.AddTransient<ContentProviderCreatedIntegrationEventHandler>();
        }
    }
}
