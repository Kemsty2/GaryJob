using Storage.Net;
using Storage.Net.Blobs;
using System;

namespace GaryJob.Core.Options
{
    public class DocumentStorageOptions
    {
        public Func<IBlobStorage> BlobStorageFactory { get; set; } = StorageFactory.Blobs.InMemory;
    }
}