namespace GaryJob.Core.Entities.FileAggregate
{
    public class FileModel
    {
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public byte[] Content { get; set; }
    }
}