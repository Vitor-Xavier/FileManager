using FileManager.DTO;
using Microsoft.AspNetCore.WebUtilities;

namespace FileManager.Services.Media
{
    public interface IMediaService
    {
        FileManagetDto ReadMediaFile(string fileName);

        FileManagetDto ReadTempFile(string fileName);

        Task<FileManagerResponseDto> UploadMediaFile(IFormFile file, CancellationToken cancellationToken = default);

        Task<FileManagerResponseDto> UploadTempFile(IFormFile file, CancellationToken cancellationToken = default);

        Task<FileManagerResponseDto> UploadLargeMediaFile(MultipartReader reader, MultipartSection section);

        Task<FileManagerResponseDto> UploadLargeTempFile(MultipartReader reader, MultipartSection section);

        Task DeleteMediaFile(string fileName, CancellationToken cancellationToken = default);
    }
}
