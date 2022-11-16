using FileManager.DTO;
using FileManager.Helpers;
using FileManager.Options;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net;

namespace FileManager.Services.Tools
{
    public class ToolService : IToolService
    {
        private readonly ToolOptions _options;

        private readonly IMemoryCache _memoryCache;

        public readonly static string LogoFileName = "logo.png";

        public ToolService(IOptionsSnapshot<ToolOptions> options,
                           IMemoryCache memoryCache)
        {
            _options = options.Value;
            _memoryCache = memoryCache;
        }

        public (byte[] byteArray, string contentType) ReadLogoFile() => _memoryCache.GetOrCreate("logo", (options) =>
        {
            options.SetPriority(CacheItemPriority.High);
            options.SlidingExpiration = TimeSpan.FromMinutes(5);
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60);

            var file = FileHelper.ReadFile(LogoFileName, _options.BasePath);
            byte[] byteArray = File.ReadAllBytes(file.FilePath);
            return (byteArray, file.ContentType);
        });

        public async Task<FileManagerResponseDto> UploadLogoFile(IFormFile file, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string trustedFileName = WebUtility.HtmlEncode(file.FileName);
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(trustedFileName, out string contentType) || !_options.AllowedLogoTypes.Contains(contentType))
                throw new BadHttpRequestException("Formato do arquivo não permitido");

            if (file.Length > _options.FileSizeLimit)
                throw new BadHttpRequestException("Tamanho do arquivo excede o limite permitido");

            var storageFile = await FileHelper.UploadFile(file, LogoFileName, _options.BasePath, cancellationToken);

            _memoryCache.Remove("logo");

            return new FileManagerResponseDto(storageFile.FileName);
        }
    }
}
