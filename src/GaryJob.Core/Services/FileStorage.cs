using System.Collections.Generic;
using CsvHelper;
using CsvHelper.Configuration;
using GaryJob.Core.Interfaces;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GaryJob.Core.Services
{
    public class FileStorage : IFileStorage
    {
        public async Task<byte[]> SaveFileAsync(string fileName, string filePath, List<dynamic> data, CancellationToken token = default)
        {
            var path = Path.Join(filePath, fileName);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";"
            };
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, config))
            {
                await csv.WriteRecordsAsync(data, token);
            }

            return await File.ReadAllBytesAsync(path, token);
        }
    }
}