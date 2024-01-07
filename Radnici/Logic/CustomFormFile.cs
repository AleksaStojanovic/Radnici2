namespace Radnici.Logic
{
    public class CustomFormFile : IFormFile
    {
        private readonly MemoryStream _memoryStream;
        private readonly string _fileName;
        private readonly string _contentType;

        public CustomFormFile(MemoryStream memoryStream, string fileName, string contentType)
        {
            _memoryStream = memoryStream;
            _fileName = fileName;
            _contentType = contentType;
            Length = memoryStream.Length;
        }

        public string ContentType => _contentType;
        public string ContentDisposition => $"form-data; name=\"file\"; filename=\"{_fileName}\"";
        public IHeaderDictionary Headers => new HeaderDictionary();
        public long Length { get; }
        public string Name => "file";
        public string FileName => _fileName;

        public void CopyTo(Stream target)
        {
            _memoryStream.Position = 0;
            _memoryStream.CopyTo(target);
        }

        public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            _memoryStream.Position = 0;
            await _memoryStream.CopyToAsync(target, cancellationToken);
        }

        public Stream OpenReadStream()
        {
            _memoryStream.Position = 0;
            return _memoryStream;
        }
    }
}
