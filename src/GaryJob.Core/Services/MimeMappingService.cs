using GaryJob.Core.Interfaces;
using Microsoft.AspNetCore.StaticFiles;

namespace GaryJob.Core.Services
{
    public class MimeMappingService : IMimeMappingService
    {
        private readonly FileExtensionContentTypeProvider _contentTypeProvider;

        public MimeMappingService(FileExtensionContentTypeProvider contentTypeProvider)
        {
            _contentTypeProvider = contentTypeProvider;
        }

        public string Map(string fileName)
        {
            if (!_contentTypeProvider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}