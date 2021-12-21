using GaryJob.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GaryJob.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .ConfigureControllers()
                .ConfigureHealthChecks(Configuration)
                .ConfigureApiVersioning()
                .ConfigureSwagger(Configuration)
                .ConfigureElsa(Configuration)
                .ConfigureCors()
                .ConfigureHangfire(Configuration)
                .ConfigureOptions(Configuration)
                .ConfigureDomainServices(Environment)
                .ConfigurePersistenceServices(Configuration)
                .AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app
                .UseAutoWrapper()
                .UseStaticFiles()
                .UseHttpActivities()
                .UseRouting()
                .UseSwagger(provider, Configuration)
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints
                        .ConfigureHealthCheck();

                    endpoints.MapFallbackToPage("/_Host");
                });
        }
    }
}