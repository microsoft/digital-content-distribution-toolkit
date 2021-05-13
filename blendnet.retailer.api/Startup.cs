using blendnet.common.dto;
using blendnet.common.dto.Retailer;
using blendnet.retailer.repository.CosmosRepository;
using blendnet.retailer.repository.Interfaces;
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

namespace blendnet.retailer.api
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
            });


            //Set up App Settings
            services.Configure<RetailerAppSettings>(Configuration);

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

            services.AddTransient<IRetailerRepository, RetailerRepository>();

            //Configure Cosmos DB
            ConfigureCosmosDB(services);

            //Configure Redis Cache
            ConfigureDistributedCache(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseForwardedHeaders();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseForwardedHeaders();
            }

            app.UseHttpsRedirection();

            app.UseCors(C_CORS_POLICYNAME);

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
                endpoints.MapHealthChecks("/health");
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

                ContainerResponse containerResponse = database.Database.CreateContainerIfNotExistsAsync(ApplicationConstants.CosmosContainers.Retailer, "/partnerId").Result;

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
    }
}
