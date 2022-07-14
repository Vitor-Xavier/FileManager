﻿using FileManager.DTO;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Net;

namespace FileManager.Helpers
{
    public static class FileHelper
    {
        public static FileManagetDto ReadFile(string fileName, string path)
        {
            string trustedFileName = WebUtility.HtmlEncode(fileName);
            string filePath = Path.Combine(path, trustedFileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Arquivo não encontrado");

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out string contentType))
                throw new BadHttpRequestException("Arquivo em formato não reconhecido");

            return new FileManagetDto(trustedFileName, filePath, contentType);
        }

        public static async Task<FileManagetDto> UploadFile(IFormFile file, string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (file.Length == 0) throw new BadHttpRequestException("Arquivo vazio");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string trustedFileName = Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(file.FileName));
            var filePath = Path.Combine(path, trustedFileName);

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out string contentType))
                throw new BadHttpRequestException("Arquivo em formato não reconhecido");

            using var stream = File.Create(filePath);
            await file.CopyToAsync(stream, cancellationToken);

            return new FileManagetDto(trustedFileName, filePath, contentType);
        }

        public static async Task<FileManagetDto> UploadLargeFile(MultipartReader reader, MultipartSection section, string path, Action<string> validateType, Action<long> validateSize)
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
                        string trustedFileName = Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(contentDisposition.FileName.Value));
                        string filePath = Path.Combine(path, trustedFileName);

                        var provider = new FileExtensionContentTypeProvider();
                        if (!provider.TryGetContentType(filePath, out string contentType))
                            throw new BadHttpRequestException("Arquivo em formato não reconhecido");
                        if (validateType is not null) validateType(trustedFileName);
                        if (validateSize is not null) validateSize(section.Body.Length);

                        byte[] fileArray;
                        using (var memoryStream = new MemoryStream())
                        {
                            await section.Body.CopyToAsync(memoryStream);
                            fileArray = memoryStream.ToArray();
                        }

                        using var fileStream = File.Create(filePath);
                        await fileStream.WriteAsync(fileArray);

                        return new FileManagetDto(trustedFileName, filePath, contentType);
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
