using Elsa.Activities.Email.Options;
using Elsa.Activities.Http.Options;
using Elsa.Server.Hangfire.Extensions;
using GaryJob.Core.Extensions;
using GaryJob.Core.Options;
using GaryJob.Workflows.Extensions;
using Hangfire;
using Hangfire.SQLite;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Storage.Net;
using System;
using System.IO;
using System.Reflection;

namespace GaryJob.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureElsa(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Sqlite");
            services.AddWorkflowServices(dbContext => dbContext.UseSqlite(connectionString));

            // Configure SMTP.
            services.Configure<SmtpOptions>(options => configuration.GetSection("Elsa:Smtp").Bind(options));

            // Configure HTTP activities.
            services.Configure<HttpActivityOptions>(options => configuration.GetSection("Elsa:Server").Bind(options));

            // Elsa API (to allow Elsa Dashboard to connect for checking workflow instances).
            services.AddElsaApiEndpoints();

            return services;
        }

        public static IServiceCollection ConfigureHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddHangfire(config => config
                    // Use same SQLite database as Elsa for storing jobs.
                    .UseSQLiteStorage(configuration.GetConnectionString("Sqlite"))
                    .UseSimpleAssemblyNameTypeSerializer()

                    // Elsa uses NodaTime primitives, so Hangfire needs to be able to serialize them.
                    .UseRecommendedSerializerSettings(settings => settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb)))
                .AddHangfireServer((sp, options) =>
                {
                    // Bind settings from configuration.
                    configuration.GetSection("Hangfire").Bind(options);

                    // Configure queues for Elsa workflow dispatchers.
                    options.ConfigureForElsaDispatchers(sp);
                });

            return services;
        }

        public static IServiceCollection ConfigureCors(this IServiceCollection services)
        {
            services
                .AddCors(cors => cors.AddDefaultPolicy(policy => policy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .WithExposedHeaders("Content-Disposition")));
            return services;
        }

        public static IServiceCollection ConfigureDomainServices(this IServiceCollection services, IWebHostEnvironment environment)
        {
            services.AddDomainServices();
            services.Configure<DocumentStorageOptions>(options => options.BlobStorageFactory = () => StorageFactory.Blobs.DirectoryFiles(Path.Combine(environment.ContentRootPath, "App_Data/Uploads")));

            return services;
        }

        public static IServiceCollection ConfigurePersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        public static IServiceCollection ConfigureControllers(this IServiceCollection services)
        {
            services
                .AddControllers(options =>
                {
                    options.RespectBrowserAcceptHeader = true;
                    options.ReturnHttpNotAcceptable = true;
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            return services;
        }

        public static IServiceCollection ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "self", "ready" });

            return services;
        }

        public static IServiceCollection ConfigureHttpContextAccessor(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            return services;
        }

        public static IServiceCollection ConfigureApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(), new HeaderApiVersionReader("api-version"));
            });

            // Add VersionedApiExplorer()
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }

        public static IServiceCollection ConfigureCookiePolicy(this IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = _ => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            return services;
        }

        public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        public static IServiceCollection ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                // note: need a temporary service provider here because one has not been created yet
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                // add a swagger document for each discovered API version
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerDoc(description.GroupName, new OpenApiInfo
                    {
                        Title = "Gaby Zhang Job",
                        Version = description.GroupName,
                        Contact = new OpenApiContact
                        {
                            Name = "Steeve Kemgne",
                            Email = "king.kemsty@gmail.com"
                        }
                    });
                    c.EnableAnnotations();
                }

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            return services;
        }
    }
}