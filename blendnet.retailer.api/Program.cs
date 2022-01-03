using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using blendnet.api.proxy;
using blendnet.api.proxy.Device;
using blendnet.api.proxy.KaizalaIdentity;
using blendnet.common.dto;
using blendnet.common.dto.Retailer;
using blendnet.common.infrastructure.ApplicationInsights;
using blendnet.common.infrastructure.Authentication;
using blendnet.common.infrastructure.Extensions;
using blendnet.common.infrastructure.KeyVault;
using blendnet.common.infrastructure.Middleware;
using blendnet.retailer.repository.CosmosRepository;
using blendnet.retailer.repository.Interfaces;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
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
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


var builder = WebApplication.CreateBuilder(args);

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
ConfigureServices(builder.Services);

var app = builder.Build();

//Invoke Configure
Configure(app, builder.Environment);

//run the app
app.Run();

#region Private Methods

// This method gets called by the runtime. Use this method to add services to the container.
void ConfigureServices(IServiceCollection services)
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

    // Kaizala Auth Setup
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
            Title = "BlendNet Retailer API",
            Description = "Web API to manage retailer on the BlendNet platform.",
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
    services.Configure<RetailerAppSettings>(builder.Configuration);

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

    services.AddTransient<IRetailerRepository, RetailerRepository>();
    services.AddTransient<IRetailerProviderRepository, RetailerProviderRepository>();

    //registerations required for authhandler to work
    services.AddTransient<KaizalaIdentityProxy>();
    services.AddTransient<UserProxy>();
    services.AddTransient<IUserDetails, UserDetailsByProxy>();

    // Configure Services
    services.AddTransient<DeviceProxy>();

    //Configure Cosmos DB
    ConfigureCosmosDB(services);

    //Configure Redis Cache
    ConfigureDistributedCache(services);

    //Configure Http Clients
    ConfigureHttpClients(services);

    //Configure automapper
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
            c.SwaggerEndpoint("v1/swagger.json", "BlendNet Retailer API V1");
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

        ContainerResponse containerResponse = database.Database.CreateContainerIfNotExistsAsync(ApplicationConstants.CosmosContainers.Retailer, ApplicationConstants.CosmosContainers.RetailerPartitionKey).Result;
        containerResponse = database.Database.CreateContainerIfNotExistsAsync(ApplicationConstants.CosmosContainers.RetailerProvider, ApplicationConstants.CosmosContainers.RetailerProviderPartitionKey).Result;

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
/// Configure Http Clients
/// </summary>
void ConfigureHttpClients(IServiceCollection services)
{
    //Configure Http Clients
    services.AddHttpClient(ApplicationConstants.HttpClientKeys.KAIZALA_HTTP_CLIENT, c =>
    {
        c.DefaultRequestHeaders.Add("Accept", "application/json");
    });

    //Configure Http Client for User Proxy
    string userBaseUrl = builder.Configuration.GetValue<string>("UserBaseUrl");
    services.AddHttpClient(ApplicationConstants.HttpClientKeys.USER_HTTP_CLIENT, c =>
    {
        c.BaseAddress = new Uri(userBaseUrl);
        c.DefaultRequestHeaders.Add("Accept", "application/json");
    });

    string deviceBaseUrl = builder.Configuration.GetValue<string>("DeviceBaseUrl");
    services.AddHttpClient(ApplicationConstants.HttpClientKeys.DEVICE_HTTP_CLIENT, c =>
    {
        c.BaseAddress = new Uri(deviceBaseUrl);
        c.DefaultRequestHeaders.Add("Accept", "application/json");
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

#endregion