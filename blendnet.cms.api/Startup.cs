using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus.Administration;
using Azure.Storage;
using Azure.Storage.Blobs;
using blendnet.cms.repository.CosmosRepository;
using blendnet.cms.repository.Interfaces;
using blendnet.common.dto;
using blendnet.common.dto.cms;
using blendnet.common.infrastructure;
using blendnet.common.infrastructure.ServiceBus;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

namespace blendnet.cms.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // configure Azure AD B2C Authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(options =>
            {
                Configuration.Bind("AzureAdB2C", options);

                options.TokenValidationParameters.NameClaimType = "name";
            },
            options => {
                Configuration.Bind("AzureAdB2C", options);
            });

            services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            //Configure Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "BlendNet CMS API",
                    Description = "Web API to manage media content on the BlendNet platform.",
                    TermsOfService = new Uri("https://api.blendnet.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "BlendNet Team",
                        Url = new Uri("https://twitter.com/teamblendnet"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under BlendNet terms",
                        Url = new Uri("https://api.blendnet.com/license"),
                    }
                });
            });

            //Set up App Settings
            services.Configure<AppSettings>(Configuration);

            //Configuring API Versiong
            services.AddApiVersioning(x =>
            {
                x.DefaultApiVersion = new ApiVersion(1, 0);

                // If user does not specified a version we consider is 1.0 version of our API
                x.AssumeDefaultVersionWhenUnspecified = true;

                x.ReportApiVersions = true;

                // If you want to read which API version the user requests from HTTP header , you can use the line below.
                //x.ApiVersionReader = new HeaderApiVersionReader ("x-starterapi-version");
            });

            //Configure Application Insights
            services.AddApplicationInsightsTelemetry();

            string cmsStorageConnectionString = Configuration.GetValue<string>("CMSStorageConnectionString");

            string cmsCDNStorageConnectionString = Configuration.GetValue<string>("CMSCDNStorageConnectionString");
            
            services.AddAzureClients(builder => 
                    {
                        // Register blob service client and initialize it using the Storage section of configuration
                        builder.AddBlobServiceClient(cmsStorageConnectionString)
                                .WithName(ApplicationConstants.StorageInstanceNames.CMSStorage)
                                .WithVersion(BlobClientOptions.ServiceVersion.V2019_02_02);

                        builder.AddBlobServiceClient(cmsCDNStorageConnectionString)
                                .WithName(ApplicationConstants.StorageInstanceNames.CMSCDNStorage)
                                .WithVersion(BlobClientOptions.ServiceVersion.V2019_02_02);

                    });
            // services.AddSingleton<BlobServiceClient>(bsc => {
                    
            //         var client = new BlobServiceClient(cmsCDNStorageConnectionString);
                    
            //         return client;
            //     });

            //Configure Services
            services.AddTransient<IContentProviderRepository, ContentProviderRepository>();

            services.AddTransient<IContentRepository, ContentRepository>();

            //Configure Cosmos DB
            ConfigureCosmosDB(services);

            //Configure Service Bus
            string serviceBusConnectionString = Configuration.GetValue<string>("ServiceBusConnectionString");

            services.AddAzureClients(builder =>
            {
                builder.AddServiceBusClient(serviceBusConnectionString);
            });

            ConfigureEventBus(services);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlendNet API V1");
            });

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// Configure Event Bus
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureEventBus(IServiceCollection services)
        {
            //event bus related registrations
            string serviceBusConnectionString = Configuration.GetValue<string>("ServiceBusConnectionString");

            string serviceBusTopicName = Configuration.GetValue<string>("ServiceBusTopicName");

            services.AddSingleton<EventBusConnectionData>(ebcd =>
            {
                EventBusConnectionData eventBusConnectionData = new EventBusConnectionData();

                eventBusConnectionData.ServiceBusConnectionString = serviceBusConnectionString;

                eventBusConnectionData.TopicName = serviceBusTopicName;

                return eventBusConnectionData;
            });

            services.AddSingleton<IEventBus, EventServiceBus>();
        }

        /// <summary>
        /// Set up Cosmos DB
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureCosmosDB(IServiceCollection services)
        {
            string account = Configuration.GetValue<string>("AccountEndPoint");

            string databaseName = Configuration.GetValue<string>("DatabaseName");

            string key = Configuration.GetValue<string>("AccountKey");

            services.AddSingleton<CosmosClient>((cc) => {

                CosmosClient client = new CosmosClientBuilder(account, key)
                           .WithSerializerOptions(new CosmosSerializationOptions() 
                            {   PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase 
                            })
                           .Build();

                DatabaseResponse database = client.CreateDatabaseIfNotExistsAsync(databaseName).Result;

                ContainerResponse containerResponse = database.Database.CreateContainerIfNotExistsAsync(ApplicationConstants.CosmosContainers.ContentProvider, "/id").Result;

                containerResponse = database.Database.CreateContainerIfNotExistsAsync(ApplicationConstants.CosmosContainers.Content, "/contentId").Result;

                return client;
            });
        }
    }
}
