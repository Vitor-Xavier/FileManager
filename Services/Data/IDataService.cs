using FileManager.DTO;
using Microsoft.AspNetCore.WebUtilities;

namespace FileManager.Services.Data
{
    public interface IDataService
    {
        FileManagetDto ReadFile(string fileName);

        FileManagetDto ReadTempFile(string fileName);

        Task<FileManagerResponseDto> UploadFile(IFormFile file, CancellationToken cancellationToken = default);

        Task<FileManagerResponseDto> UploadTempFile(IFormFile file, CancellationToken cancellationToken = default);

        Task<FileManagerResponseDto> UploadLargeFile(MultipartReader reader, MultipartSection section);

        Task<FileManagerResponseDto> UploadLargeTempFile(MultipartReader reader, MultipartSection section);

        void DeleteFile(string fileName);
    }
}
