using GaryJob.Core.Interfaces;
using GaryJob.Core.Options;
using GaryJob.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Storage.Net;

namespace GaryJob.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.Configure<DocumentStorageOptions>(options => options.BlobStorageFactory = StorageFactory.Blobs.InMemory);

            return services
                .AddSingleton<IFileStorage, FileStorage>();
        }
    }
}