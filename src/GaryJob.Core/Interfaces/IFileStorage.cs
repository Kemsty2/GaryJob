using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GaryJob.Core.Interfaces
{
    public interface IFileStorage
    {
        Task<byte[]> SaveFileAsync(string fileName, string filePath, List<dynamic> data, CancellationToken token = default);
    }
}