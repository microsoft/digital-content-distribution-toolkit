using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using System;
using Serilog;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using blendnet.crm.retailer.repository;
using blendnet.crm.retailer.repository.Interfaces;
using blendnet.crm.retailer.repository.CosmosRepository;

namespace blendnet.crm.retailer.api
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddMicrosoftIdentityWebApi(options =>
           {
               Configuration.Bind("AzureAdB2C", options);

               options.TokenValidationParameters.NameClaimType = "name";
           },
           options => { 
               Configuration.Bind("AzureAdB2C", options); 
           });
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "BlendNet Retailer API",
                    Description = "Web API to manage the Retailers on the BlendNet platform.",
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

            //Configure Cosmos DB context
            string accountEndPoint = Configuration.GetValue<string>("AccountEndPoint");
            
            string accountKey = Configuration.GetValue<string>("AccountKey");
            
            string databaseName = Configuration.GetValue<string>("DatabaseName");
            
            //Configure Cosmos DB context
            services.AddDbContext<BlendNetContext>(options =>
            
            options.UseCosmos(accountEndPoint,accountKey,databaseName));

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

            //Configure Services
            services.AddTransient<IRetailerRepository, RetailerRepository>();
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

            app.UseSwagger();

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
            using (var serviceScope = app.ApplicationServices
              .GetRequiredService<IServiceScopeFactory>()
              .CreateScope())
            {
                var blendNetContext = serviceScope.ServiceProvider.GetService<BlendNetContext>();

                blendNetContext.Database.EnsureCreated();
            };
        }
    }
}
