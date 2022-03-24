using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using blendnet.api.proxy;
using blendnet.api.proxy.Cms;
using blendnet.api.proxy.KaizalaIdentity;
using blendnet.api.proxy.Retailer;
using blendnet.common.dto;
using blendnet.common.dto.Incentive;
using blendnet.common.infrastructure;
using blendnet.common.infrastructure.ApplicationInsights;
using blendnet.common.infrastructure.Authentication;
using blendnet.common.infrastructure.Extensions;
using blendnet.common.infrastructure.KeyVault;
using blendnet.common.infrastructure.Middleware;
using blendnet.common.infrastructure.ServiceBus;
using blendnet.incentive.api;
using blendnet.incentive.api.Common;
using blendnet.incentive.repository.IncentiveRepository;
using blendnet.incentive.repository.Interfaces;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//remove server header
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.AddServerHeader = false;
});


const string C_CORS_POLICYNAME = "BlendNetSpecificOrigins";

Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

//read the configuration from keyvault in case of production
////https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-5.0
if (builder.Environment.IsProduction())
{
    //configure application configuration
    var secretClient = new SecretClient(
        new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
        new DefaultAzureCredential());

    builder.Configuration.AddAzureKeyVault(secretClient, new PrefixKeyVaultSecretManager(builder.Configuration["KeyVaultPrefix"]));
}

builder.Logging.ClearProviders();

builder.Logging.AddConsole();

builder.Logging.AddDebug();

builder.Logging.AddSerilog();

//Configure Services
ConfigureServices(builder);

var app = builder.Build();

//Invoke Configure
Configure(app, builder.Environment);

//run the app
app.Run();

#region Private Methods

// This method gets called by the runtime. Use this method to add services to the container.
void ConfigureServices(WebApplicationBuilder builder)
{
    IServiceCollection services = builder.Services;

    // Read AllowedCORSUrls list
    List<string> corsUrls = builder.Configuration.GetSection("AllowedCORSUrls").Get<List<string>>();

    services.AddCors(options =>
    {
        options.AddPolicy(name: C_CORS_POLICYNAME,
                          builder =>
                          {
                              if (corsUrls != null && corsUrls.Count > 0)
                              {
                                  builder.WithOrigins(corsUrls.ToArray())
                                         .WithMethods(ApplicationConstants.AllowedCORS_Headers)
                                         .AllowAnyHeader();

                              }
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
        .AddDataAnnotationsLocalization(options =>
        {
            options.DataAnnotationLocalizerProvider = (type, factory) =>
                factory.Create(typeof(SharedResource));
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
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
            Title = "BlendNet Incentive API",
            Description = "Web API to manage incentives on the BlendNet platform.",
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
    services.Configure<IncentiveAppSettings>(builder.Configuration);

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
    services.AddSingleton<ITelemetryInitializer, BlendNetTelemetryInitializer>();
    services.AddApplicationInsightsTelemetry();

    //Configure health check
    services.AddHealthChecks();

    //Configure Services
    services.AddTransient<IIncentiveRepository, IncentiveRepository>();
    services.AddTransient<IEventRepository, EventRepository>();
    services.AddTransient<RetailerProviderProxy>();
    services.AddTransient<RetailerProxy>();
    services.AddTransient<IncentiveCalculationHelper>();
    services.AddTransient<ContentProxy>();
    services.AddTransient<ContentProviderProxy>();

    //registerations required for authhandler to work
    services.AddTransient<KaizalaIdentityProxy>();
    services.AddTransient<UserProxy>();
    services.AddTransient<IUserDetails, UserDetailsByProxy>();

    //Configure Cosmos DB
    ConfigureCosmosDB(services);

    //Configure Http Clients
    ConfigureHttpClients(services);

    //Configure Redis Cache
    ConfigureDistributedCache(services);

    // Configure Event Bus
    ConfigureEventBus(services);

    // Configure mapper
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
}

// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseForwardedHeaders();
    }
    else
    {
        app.UseExceptionHandler(errorApp =>
        {
            ILogger<Program> logger = app.ApplicationServices.GetService<ILogger<Program>>();

            errorApp.RunCustomGlobalExceptionHandler(logger);
        });

        app.UseForwardedHeaders();
    }

    // Add other security headers
    app.UseMiddleware<SecurityHeadersMiddleware>();

    app.UseHttpsRedirection();

    app.UseCors(C_CORS_POLICYNAME);

    //To allow accepting language header
    var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
    app.UseRequestLocalization(options.Value);

    bool swaggerEnabled = env.IsDevelopment() || builder.Configuration.GetValue<bool>("SwaggerDocEnabled");
    if (swaggerEnabled)
    {
        // Enable middleware to serve generated Swagger as a JSON endpoint.
        app.UseSwagger();

        // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
        // specifying the Swagger JSON endpoint.
        app.UseSwaggerUI(c =>
        {
            //c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlendNet API V1");
            c.SwaggerEndpoint("v1/swagger.json", "BlendNet Incentive API V1");
        });
    }

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
void ConfigureHttpClients(IServiceCollection services)
{
    //Configure Http Clients
    services.AddHttpClient(ApplicationConstants.HttpClientKeys.KAIZALA_HTTP_CLIENT, c =>
    {
        c.DefaultRequestHeaders.Add("Accept", "application/json");
    });

    //Configure Http Clients
    services.AddHttpClient(ApplicationConstants.HttpClientKeys.RETAILER_HTTP_CLIENT, c =>
    {
        string retailerBaseUrl = builder.Configuration.GetValue<string>("RetailerBaseUrl");
        c.BaseAddress = new Uri(retailerBaseUrl);
        c.DefaultRequestHeaders.Add("Accept", "application/json");
    });

    //Configure Http Clients
    services.AddHttpClient(ApplicationConstants.HttpClientKeys.CMS_HTTP_CLIENT, c =>
    {
        string cmsBaseUrl = builder.Configuration.GetValue<string>("CmsBaseUrl");
        c.BaseAddress = new Uri(cmsBaseUrl);
        c.DefaultRequestHeaders.Add("Accept", "application/json");
    });

    //Configure Http Client for User Proxy
    string userBaseUrl = builder.Configuration.GetValue<string>("UserBaseUrl");
    services.AddHttpClient(ApplicationConstants.HttpClientKeys.USER_HTTP_CLIENT, c =>
    {
        c.BaseAddress = new Uri(userBaseUrl);
        c.DefaultRequestHeaders.Add("Accept", "application/json");
    });
}

/// <summary>
/// Set up Cosmos DB
/// </summary>
/// <param name="services"></param>
void ConfigureCosmosDB(IServiceCollection services)
{
    string account = builder.Configuration.GetValue<string>("AccountEndPoint");

    string databaseName = builder.Configuration.GetValue<string>("DatabaseName");

    string key = builder.Configuration.GetValue<string>("AccountKey");

    services.AddSingleton<CosmosClient>((cc) =>
    {

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
/// Configures Redis as distributed cache
/// </summary>
/// <param name="services"></param>
void ConfigureDistributedCache(IServiceCollection services)
{
    string redisCacheConnectionString = builder.Configuration.GetValue<string>("RedisCacheConnectionString");

    services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisCacheConnectionString;
    });

}

/// <summary>
/// Configure Localization
/// </summary>
void ConfigureLocalization(IServiceCollection services)
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


/// <summary>
/// Configure Event Bus
/// </summary>
void ConfigureEventBus(IServiceCollection services)
{
    //event bus related registrations
    string serviceBusConnectionString = builder.Configuration.GetValue<string>("ServiceBusConnectionString");
    string serviceBusTopicName = builder.Configuration.GetValue<string>("ServiceBusTopicName");

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

#endregion