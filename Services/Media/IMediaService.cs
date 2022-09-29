using FileManager.DTO;
using Microsoft.AspNetCore.WebUtilities;
using FileAccess = FileManager.Models.FileAccess;

namespace FileManager.Services.Media
{
    public interface IMediaService
    {
        Task<FileManagerDto> ReadMediaFile(string fileName, CancellationToken cancellationToken = default);

        FileManagerDto ReadTempFile(string fileName);

        Task<FileManagerResponseDto> UploadMediaFile(IFormFile file, FileAccess access, CancellationToken cancellationToken = default);

        Task<FileManagerResponseDto> UploadTempFile(IFormFile file, CancellationToken cancellationToken = default);

        Task<FileManagerResponseDto> UploadLargeMediaFile(MultipartReader reader, MultipartSection section);

        Task<FileManagerResponseDto> UploadLargeTempFile(MultipartReader reader, MultipartSection section);

        Task DeleteMediaFile(string fileName, CancellationToken cancellationToken = default);
    }
}
