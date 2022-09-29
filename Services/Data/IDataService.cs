using FileManager.DTO;
using Microsoft.AspNetCore.WebUtilities;
using FileAccess = FileManager.Models.FileAccess;

namespace FileManager.Services.Data
{
    public interface IDataService
    {
        Task<FileManagerDto> ReadDataFile(string fileName, CancellationToken cancellationToken = default);

        FileManagerDto ReadTempFile(string fileName);

        Task<FileManagerResponseDto> UploadDataFile(IFormFile file, FileAccess access, CancellationToken cancellationToken = default);

        Task<FileManagerResponseDto> UploadTempFile(IFormFile file, CancellationToken cancellationToken = default);

        Task<FileManagerResponseDto> UploadLargeDataFile(MultipartReader reader, MultipartSection section);

        Task<FileManagerResponseDto> UploadLargeTempFile(MultipartReader reader, MultipartSection section);

        Task DeleteDataFile(string fileName, CancellationToken cancellationToken = default);
    }
}
