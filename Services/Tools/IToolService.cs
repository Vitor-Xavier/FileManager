using FileManager.DTO;

namespace FileManager.Services.Tools
{
    public interface IToolService
    {
        (byte[] byteArray, string contentType) ReadLogoFile();

        Task<FileManagerResponseDto> UploadLogoFile(IFormFile file, CancellationToken cancellationToken = default);
    }
}
