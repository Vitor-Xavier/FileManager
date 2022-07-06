using FileManager.DTO;
using FileManager.Helpers;
using FileManager.Models;
using FileManager.Options;
using FileManager.Repositories.StorageFiles;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Net;

namespace FileManager.Services.Data
{
    public class DataService : IDataService
    {
        private readonly DataOptions _options;

        private readonly IStorageFileRepository _storageFileRepository;

        public DataService(IOptionsSnapshot<DataOptions> options, IStorageFileRepository storageFileRepository)
        {
            _options = options.Value;
            _storageFileRepository = storageFileRepository;
        }

        public FileManagetDto ReadDataFile(string fileName) => ReadFile(fileName, _options.BasePath);

        public FileManagetDto ReadTempFile(string fileName) => ReadFile(fileName, _options.TempPath);

        public async Task<FileManagerResponseDto> UploadDataFile(IFormFile file, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fileData = await UploadFile(file, _options.BasePath, cancellationToken);
            StorageFile storageFile = new()
            {
                FileName = fileData.FileName,
                Path = fileData.FilePath,
                Owner = "default",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Deleted = false,
            };
            await _storageFileRepository.Add(storageFile, CancellationToken.None);

            return new FileManagerResponseDto(storageFile.FileName);
        }

        public async Task<FileManagerResponseDto> UploadTempFile(IFormFile file, CancellationToken cancellationToken = default)
        {
            var fileData = await UploadFile(file, _options.BasePath, cancellationToken);
            return new FileManagerResponseDto(fileData.FileName);
        }

        public async Task<FileManagerResponseDto> UploadLargeDataFile(MultipartReader reader, MultipartSection section)
        {
            var fileData = await UploadLargeFile(reader, section, _options.BasePath);
            StorageFile storageFile = new()
            {
                FileName = fileData.FileName,
                Path = fileData.FilePath,
                Owner = "default",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Deleted = false,
            };
            await _storageFileRepository.Add(storageFile, CancellationToken.None);

            return new FileManagerResponseDto(storageFile.FileName);
        }

        public async Task<FileManagerResponseDto> UploadLargeTempFile(MultipartReader reader, MultipartSection section)
        {
            var fileData = await UploadLargeFile(reader, section, _options.BasePath);
            return new FileManagerResponseDto(fileData.FileName);
        }

        public async Task DeleteDataFile(string fileName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            DeleteFile(fileName, _options.BasePath);
            var storageFile = await _storageFileRepository.GetStorageFileByName(fileName, CancellationToken.None);
            await _storageFileRepository.Delete(storageFile, CancellationToken.None);
        }

        private FileManagetDto ReadFile(string fileName, string path) => FileHelper.ReadFile(fileName, path);

        private async Task<FileManagetDto> UploadFile(IFormFile file, string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string trustedFileName = WebUtility.HtmlEncode(file.FileName);
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(trustedFileName, out string contentType) || !_options.AllowedTypes.Contains(contentType))
                throw new BadHttpRequestException("Formato do arquivo não permitido");

            return await FileHelper.UploadFile(file, path, cancellationToken);
        }

        private async Task<FileManagetDto> UploadLargeFile(MultipartReader reader, MultipartSection section, string path)
        {
            void validateType(string contentType)
            {
                if (!_options.AllowedTypes.Contains(contentType))
                    throw new BadHttpRequestException("Formato do arquivo não permitido");
            }

            return await FileHelper.UploadLargeFile(reader, section, path, validateType);
        }

        private void DeleteFile(string fileName, string path) => FileHelper.DeleteFile(fileName, path);
    }
}
