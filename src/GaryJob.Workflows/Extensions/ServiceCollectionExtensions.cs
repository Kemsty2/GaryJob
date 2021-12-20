using Elsa;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Providers.Workflows;
using Elsa.Server.Hangfire.Extensions;
using GaryJob.Workflows.Activities;
using GaryJob.Workflows.Handlers;
using GaryJob.Workflows.Scripting.JavaScript;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Storage.Net;
using System;
using System.IO;

namespace GaryJob.Workflows.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWorkflowServices(this IServiceCollection services, Action<DbContextOptionsBuilder> configureDb)
        {
            return services
                .AddElsa(configureDb);
        }

        private static IServiceCollection AddElsa(this IServiceCollection services, Action<DbContextOptionsBuilder> configureDb)
        {
            services
                .AddElsa(elsa => elsa

                    // Use EF Core's SQLite provider to store workflow instances and bookmarks.
                    .UseEntityFrameworkPersistence(configureDb)

                    // Ue Console activities for testing & demo purposes.
                    .AddConsoleActivities()

                    // Use Hangfire to dispatch workflows from.
                    .UseHangfireDispatchers()

                    // Configure Email activities.
                    .AddEmailActivities()

                    // Configure HTTP activities.
                    .AddHttpActivities()

                    .AddActivitiesFrom<SaveFile>()
                );

            var currentAssemblyPath = Path.GetDirectoryName(typeof(ServiceCollectionExtensions).Assembly.Location);

            // Configure Storage for BlobStorageWorkflowProvider with a directory on disk from where to load workflow definition JSON files from the local "Workflows" folder.
            services.Configure<BlobStorageWorkflowProviderOptions>(options => options.BlobStorageFactory = () => StorageFactory.Blobs.DirectoryFiles(Path.Combine(currentAssemblyPath!, "Workflows")));

            services.AddNotificationHandlersFrom<StartSaveFileWorkflowHandler>();

            // Register custom type definition provider for JS intellisense.
            services.AddJavaScriptTypeDefinitionProvider<CustomTypeDefinitionProvider>();

            return services;
        }
    }
}