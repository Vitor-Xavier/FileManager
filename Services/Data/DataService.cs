using FileManager.DTO;
using FileManager.Helpers;
using FileManager.Options;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace FileManager.Services.Data
{
    public class DataService : IDataService
    {
        private readonly DataOptions _options;

        public DataService(IOptionsSnapshot<DataOptions> options)
        {
            _options = options.Value;
        }

        public FileManagetDto ReadFile(string fileName) => FileHelper.ReadFile(fileName, _options.BasePath);

        public FileManagetDto ReadTempFile(string fileName) => FileHelper.ReadFile(fileName, _options.TempPath);

        public async Task<FileManagerResponseDto> UploadFile(IFormFile file, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fileData = await FileHelper.UploadFile(file, _options.BasePath, cancellationToken);

            return new FileManagerResponseDto(fileData.FileName);
        }

        public async Task<FileManagerResponseDto> UploadTempFile(IFormFile file, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fileData = await FileHelper.UploadFile(file, _options.TempPath, cancellationToken);

            return new FileManagerResponseDto(fileData.FileName);
        }

        public async Task<FileManagerResponseDto> UploadLargeFile(MultipartReader reader, MultipartSection section)
        {
            var fileData = await FileHelper.UploadLargeFile(reader, section, _options.BasePath);

            return new FileManagerResponseDto(fileData.FileName);
        }

        public async Task<FileManagerResponseDto> UploadLargeTempFile(MultipartReader reader, MultipartSection section)
        {
            var fileData = await FileHelper.UploadLargeFile(reader, section, _options.TempPath);

            return new FileManagerResponseDto(fileData.FileName);
        }

        public void DeleteFile(string fileName) => FileHelper.DeleteFile(fileName, _options.BasePath);
    }
}
