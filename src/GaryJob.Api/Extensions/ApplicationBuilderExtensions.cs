using AutoWrapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;

namespace GaryJob.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, IApiVersionDescriptionProvider provider, IConfiguration configuration)
        {
            // Enable middleware to serve generated Swagger as JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });

            return app;
        }

        public static IApplicationBuilder UseAutoWrapper(this IApplicationBuilder app)
        {
            app.UseApiResponseAndExceptionWrapper(new AutoWrapperOptions
            {
                ShouldLogRequestData = false,
                UseApiProblemDetailsException = false,
                EnableResponseLogging = false,
                EnableExceptionLogging = true,
                IsApiOnly = false
            });
            return app;
        }

        public static IEndpointRouteBuilder ConfigureHealthCheck(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("/readiness", new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains("ready")
            });
            endpoints.MapHealthChecks("/liveness", new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains("self")
            });

            return endpoints;
        }
    }
}