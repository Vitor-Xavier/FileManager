using FileManager.DTO;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading;

namespace FileManager.Helpers
{
    public static class FileHelper
    {
        public static FileManagerDto ReadFile(string fileName, string path)
        {
            string trustedFileName = WebUtility.HtmlEncode(fileName);
            string filePath = Path.Combine(path, trustedFileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Arquivo não encontrado");

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out string contentType))
                throw new BadHttpRequestException("Arquivo em formato não reconhecido");

            return new FileManagerDto(trustedFileName, filePath, contentType);
        }

        public static async Task<FileManagerPostDto> UploadFile(IFormFile file, string fileName, string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (file.Length == 0) throw new BadHttpRequestException("Arquivo vazio");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string trustedFileName = Path.ChangeExtension(fileName, Path.GetExtension(file.FileName));
            var filePath = Path.Combine(path, trustedFileName);

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out string contentType))
                throw new BadHttpRequestException("Arquivo em formato não reconhecido");

            using var stream = File.Create(filePath);
            using var mySHA256 = SHA256.Create();
            await file.CopyToAsync(stream, cancellationToken);

            await stream.FlushAsync(CancellationToken.None);
            stream.Position = 0;

            return new FileManagerPostDto(trustedFileName, filePath, contentType, await mySHA256.ComputeHashAsync(stream, CancellationToken.None));
        }

        public static async Task<FileManagerPostDto> UploadLargeFile(MultipartReader reader, MultipartSection section, string fileName, string path, Action<string> validateType, Action<long> validateSize)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (contentDisposition.DispositionType.Equals("form-data") &&
                        (!string.IsNullOrEmpty(contentDisposition.FileName.Value) ||
                        !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value)))
                    {
                        string trustedFileName = Path.ChangeExtension(fileName, Path.GetExtension(contentDisposition.FileName.Value));
                        string filePath = Path.Combine(path, trustedFileName);

                        var provider = new FileExtensionContentTypeProvider();
                        if (!provider.TryGetContentType(filePath, out string contentType))
                            throw new BadHttpRequestException("Arquivo em formato não reconhecido");
                        if (validateType is not null) validateType(contentType);
                        if (validateSize is not null) validateSize(section.Body.Length);

                        byte[] fileArray;
                        using (var memoryStream = new MemoryStream())
                        {
                            await section.Body.CopyToAsync(memoryStream);
                            fileArray = memoryStream.ToArray();
                        }

                        using var fileStream = File.Create(filePath);
                        using var mySHA256 = SHA256.Create();
                        await fileStream.WriteAsync(fileArray);

                        await fileStream.FlushAsync();
                        fileStream.Position = 0;

                        return new FileManagerPostDto(trustedFileName, filePath, contentType, await mySHA256.ComputeHashAsync(fileStream));
                    }
                }
                section = await reader.ReadNextSectionAsync();
            }

            throw new BadHttpRequestException("Erro ao carregar arquivo");
        }

        public static void DeleteFile(string fileName, string path)
        {
            string trustedFileName = WebUtility.HtmlEncode(fileName);
            string filePath = Path.Combine(path, trustedFileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Arquivo não encontrado");

            File.Delete(filePath);
        }
    }
}
