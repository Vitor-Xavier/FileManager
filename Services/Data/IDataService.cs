using FileManager.DTO;
using Microsoft.AspNetCore.WebUtilities;

namespace FileManager.Services.Data
{
    public interface IDataService
    {
        FileManagetDto ReadDataFile(string fileName);

        FileManagetDto ReadTempFile(string fileName);

        Task<FileManagerResponseDto> UploadDataFile(IFormFile file, CancellationToken cancellationToken = default);

        Task<FileManagerResponseDto> UploadTempFile(IFormFile file, CancellationToken cancellationToken = default);

        Task<FileManagerResponseDto> UploadLargeDataFile(MultipartReader reader, MultipartSection section);

        Task<FileManagerResponseDto> UploadLargeTempFile(MultipartReader reader, MultipartSection section);

        Task DeleteDataFile(string fileName, CancellationToken cancellationToken = default);
    }
}
