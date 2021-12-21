using GaryJob.Core.Interfaces;
using GaryJob.Core.Options;
using GaryJob.Core.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Storage.Net;

namespace GaryJob.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.Configure<DocumentStorageOptions>(options => options.BlobStorageFactory = StorageFactory.Blobs.InMemory);
            services.AddMimeMapping();

            return services
                .AddSingleton<IFileStorage, FileStorage>();
        }

        private static IServiceCollection AddMimeMapping(this IServiceCollection services)
        {
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings.Add(".dnct", "application/dotnetcoretutorials");

            services.AddSingleton<IMimeMappingService>(new MimeMappingService(provider));

            return services;
        }
    }
}