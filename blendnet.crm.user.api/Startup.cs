using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blendnet.crm.user.api.Model;
using blendnet.crm.user.api.Repository.GraphRepository;
using blendnet.crm.user.api.Repository.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using Microsoft.Graph.Auth;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using blendnet.common.infrastructure;
using blendnet.common.infrastructure.ServiceBus;
using blendnet.common.dto.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.InteropServices.WindowsRuntime;

namespace blendnet.crm.user.api
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
                options.TokenValidationParameters.RoleClaimType = "groups";
            },
            options => {
                Configuration.Bind("AzureAdB2C", options);
            });

            services.AddControllers();

            //Configure Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "BlendNet User API",
                    Description = "Web API to manage the Users and Roles on the Azure AD B2C.",
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

            //Configure Application Settings
            services.Configure<AppSettings>(Configuration);

            //Configure Services
            services.AddTransient<IIdentityRespository, IdentityRepository>();

            //configure event bus
            ConfigureEventBus(services);

            //Configure Microsoft Graph Client
            ConfigureGraphClient(services);

        }


        /// <summary>
        /// Configure Microsoft Graph
        /// </summary>
        /// <param name="context"></param>
        /// <param name="services"></param>
        private void ConfigureGraphClient(IServiceCollection services)
        {
            string graphClientId = Configuration.GetValue<string>("GraphClientId");

            string graphClientTenant = Configuration.GetValue<string>("GraphClientTenant");

            string graphClientSecret = Configuration.GetValue<string>("GraphClientSecret");

            //Register graph authentication provider
            services.AddTransient<IAuthenticationProvider>(iap => {

                IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
               .Create(graphClientId)
               .WithTenantId(graphClientTenant)
               .WithClientSecret(graphClientSecret)
               .Build();

                ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);

                return authProvider;
            });

            //register graph
            services.AddTransient<GraphServiceClient>();
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

            string serviceBusSubscriptionName = Configuration.GetValue<string>("ServiceBusSubscriptionName");

            int serviceBusMaxConcurrentCalls = Configuration.GetValue<int>("ServiceBusMaxConcurrentCalls");

            services.AddSingleton<EventBusConnectionData>(ebcd =>
            {
                EventBusConnectionData eventBusConnectionData = new EventBusConnectionData();

                eventBusConnectionData.ServiceBusConnectionString = serviceBusConnectionString;

                eventBusConnectionData.TopicName = serviceBusTopicName;

                //set this only if you want to consume.
                //eventBusConnectionData.SubscriptionName = serviceBusSubscriptionName;
                //eventBusConnectionData.MaxConcurrentCalls = serviceBusMaxConcurrentCalls;

                return eventBusConnectionData;
            });

            services.AddSingleton<IEventBus, EventServiceBus>();

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
    }
}
