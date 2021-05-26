using blendnet.api.proxy.KaizalaIdentity;
using blendnet.api.proxy.Retailer;
using blendnet.common.dto;
using blendnet.common.dto.User;
using blendnet.common.infrastructure.Authentication;
using blendnet.user.repository.CosmosRepository;
using blendnet.user.repository.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using blendnet.common.infrastructure.ServiceBus;
using Microsoft.Extensions.Azure;
using blendnet.common.infrastructure;
using Microsoft.Extensions.Options;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace blendnet.user.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private const string C_CORS_POLICYNAME = "BlendNetSpecificOrigins";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: C_CORS_POLICYNAME,
                                  builder =>
                                  {
                                      //To Do: Remove any Origin and have right value
                                      //builder.WithOrigins("http://example.com",
                                      //                    "http://www.contoso.com");
                                      builder.AllowAnyHeader();
                                      builder.AllowAnyMethod();
                                      builder.AllowAnyOrigin();
                                  });
            });

            //Kaizala Auth Setup
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = KaizalaIdentityAuthOptions.DefaultScheme;
                options.DefaultChallengeScheme = KaizalaIdentityAuthOptions.DefaultScheme;
            })
            .AddKaizalaIdentityAuth();

            //Configure Localization
            ConfigureLocalization(services);

            services.AddControllers()
           .AddJsonOptions(options =>
           {
               options.JsonSerializerOptions.IgnoreNullValues = true;
               options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
           });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            //Configure Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "BlendNet User API",
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

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    Description = "Please insert JWT token into field"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });


            //Set up App Settings
            services.Configure<UserAppSettings>(Configuration);

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

            //Configure health check
            services.AddHealthChecks();

            //Configure Services
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<KaizalaIdentityProxy>();
            services.AddTransient<RetailerProxy>();

            //Configure Cosmos DB
            ConfigureCosmosDB(services);

            //Configure Http Clients
            ConfigureHttpClients(services);

            //Configure Redis Cache
            ConfigureDistributedCache(services);

            // Configure Event Bus
            ConfigureEventBus(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseForwardedHeaders();
            }else
            {
                app.UseExceptionHandler("/error");
                app.UseForwardedHeaders();
            }

            app.UseHttpsRedirection();

            app.UseCors(C_CORS_POLICYNAME);

            //To allow accepting language header
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                //c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlendNet User API V1");
                c.SwaggerEndpoint("v1/swagger.json", "BlendNet User API V1");
            });


            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }

        /// <summary>
        /// Configure Required Http Clients
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureHttpClients(IServiceCollection services)
        {
            //Configure Http Clients
            services.AddHttpClient(ApplicationConstants.HttpClientKeys.KAIZALAIDENTITY_HTTP_CLIENT, c =>
            {
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            //Configure Http Clients
            services.AddHttpClient(ApplicationConstants.HttpClientKeys.RETAILER_HTTP_CLIENT, c =>
            {
                string retailerBaseUrl = Configuration.GetValue<string>("RetailerBaseUrl");
                c.BaseAddress = new Uri(retailerBaseUrl);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });
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
                           {
                               PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                           })
                           .Build();

                DatabaseResponse database = client.CreateDatabaseIfNotExistsAsync(databaseName).Result;
                ContainerResponse containerResponse = database.Database.CreateContainerIfNotExistsAsync(ApplicationConstants.CosmosContainers.User, "/phoneNumber").Result;

                return client;
            });
        }


        /// <summary>
        /// Configures Redis as distributed cache
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureDistributedCache(IServiceCollection services)
        {
            string redisCacheConnectionString = Configuration.GetValue<string>("RedisCacheConnectionString");

            services.AddStackExchangeRedisCache(options => {
                options.Configuration = redisCacheConnectionString;
            });

        }

        private void ConfigureEventBus(IServiceCollection services)
        {
            //event bus related registrations
            string serviceBusConnectionString = Configuration.GetValue<string>("ServiceBusConnectionString");
            string serviceBusTopicName = Configuration.GetValue<string>("ServiceBusTopicName");

            services.AddAzureClients(builder =>
            {
                builder.AddServiceBusClient(serviceBusConnectionString);
            });

            services.AddSingleton<EventBusConnectionData>(ebcd =>
            {
                EventBusConnectionData eventBusConnectionData = new EventBusConnectionData();
                eventBusConnectionData.ServiceBusConnectionString = serviceBusConnectionString;
                eventBusConnectionData.TopicName = serviceBusTopicName;
                return eventBusConnectionData;
            });

            services.AddSingleton<IEventBus, EventServiceBus>();
        }


        private void ConfigureLocalization(IServiceCollection services)
        {
            //Add localization
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<RequestLocalizationOptions>(
               opts =>
               {
                   var supportedCultures = new List<CultureInfo>
                   {
                        new CultureInfo("en-US")
                   };

                   opts.DefaultRequestCulture = new RequestCulture("en-US");

                   // Formatting numbers, dates, etc.
                   opts.SupportedCultures = supportedCultures;

                   // UI strings that we have localized.
                   opts.SupportedUICultures = supportedCultures;

               });
        }
    }
}
