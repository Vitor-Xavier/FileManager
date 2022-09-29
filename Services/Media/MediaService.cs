using FileManager.DTO;
using FileManager.Helpers;
using FileManager.Models;
using FileManager.Options;
using FileManager.Repositories.StorageFiles;
using FileManager.Repositories.Users;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Claims;
using FileAccess = FileManager.Models.FileAccess;

namespace FileManager.Services.Media
{
    public class MediaService : IMediaService
    {
        private readonly MediaOptions _options;

        private readonly IStorageFileRepository _storageFileRepository;

        private readonly IUserRepository _userRepository;

        private readonly IHttpContextAccessor _accessor;

        public MediaService(IOptionsSnapshot<MediaOptions> options,
                            IStorageFileRepository storageFileRepository,
                            IUserRepository userRepository,
                            IHttpContextAccessor accessor)
        {
            _options = options.Value;
            _storageFileRepository = storageFileRepository;
            _userRepository = userRepository;
            _accessor = accessor;
        }

        #region Read
        public async Task<FileManagerDto> ReadMediaFile(string fileName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string username = _accessor?.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

            var storageFile = await _storageFileRepository.GetStorageFileAccessByName(fileName, cancellationToken);
            if (username != storageFile.Owner && storageFile.Access == FileAccess.Private)
                throw new FieldAccessException("Usuário sem permissão para acessar o arquivo");

            return ReadFile(fileName, _options.BasePath);
        }

        public FileManagerDto ReadTempFile(string fileName) => ReadFile(fileName, _options.TempPath);

        private static FileManagerDto ReadFile(string fileName, string path)
        {
            string trustedFileName = WebUtility.HtmlEncode(fileName);
            return FileHelper.ReadFile(trustedFileName, path);
        }
        #endregion

        #region Upload
        public async Task<FileManagerResponseDto> UploadMediaFile(IFormFile file, FileAccess access, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fileData = await UploadFile(file, _options.BasePath, cancellationToken);

            string username = _accessor?.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userRepository.GetUserByUsername(username, CancellationToken.None);

            StorageFile storageFile = new()
            {
                FileName = fileData.FileName,
                Path = fileData.FilePath,
                Hash = fileData.Hash,
                UserId = user.UserId,
                Access = access,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Deleted = false,
            };
            await _storageFileRepository.Add(storageFile, CancellationToken.None);

            return new FileManagerResponseDto(storageFile.FileName);
        }

        public async Task<FileManagerResponseDto> UploadTempFile(IFormFile file, CancellationToken cancellationToken = default)
        {
            var fileData = await UploadFile(file, _options.TempPath, cancellationToken);
            return new FileManagerResponseDto(fileData.FileName);
        }

        private async Task<FileManagerPostDto> UploadFile(IFormFile file, string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string trustedFileName = WebUtility.HtmlEncode(file.FileName);
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(trustedFileName, out string contentType) || !_options.AllowedTypes.Contains(contentType))
                throw new BadHttpRequestException("Formato do arquivo não permitido");

            if (file.Length > _options.FileSizeLimit)
                throw new BadHttpRequestException("Tamanho do arquivo excede o limite permitido");

            return await FileHelper.UploadFile(file, path, cancellationToken);
        }
        #endregion

        #region Upload Stream
        public async Task<FileManagerResponseDto> UploadLargeMediaFile(MultipartReader reader, MultipartSection section)
        {
            var fileData = await UploadLargeFile(reader, section, _options.BasePath);

            string username = _accessor?.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userRepository.GetUserByUsername(username);

            StorageFile storageFile = new()
            {
                FileName = fileData.FileName,
                Path = fileData.FilePath,
                UserId = user.UserId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Deleted = false,
            };
            await _storageFileRepository.Add(storageFile, CancellationToken.None);

            return new FileManagerResponseDto(storageFile.FileName);
        }

        public async Task<FileManagerResponseDto> UploadLargeTempFile(MultipartReader reader, MultipartSection section)
        {
            var fileData = await UploadLargeFile(reader, section, _options.TempPath);
            return new FileManagerResponseDto(fileData.FileName);
        }

        private async Task<FileManagerPostDto> UploadLargeFile(MultipartReader reader, MultipartSection section, string path)
        {
            void validateType(string contentType)
            {
                if (!_options.AllowedTypes.Contains(contentType))
                    throw new BadHttpRequestException("Formato do arquivo não permitido");
            }
            void validateSize(long fileSize)
            {
                if (fileSize > _options.FileSizeLimit)
                    throw new BadHttpRequestException("Tamanho do arquivo excede o permitido");
            }

            return await FileHelper.UploadLargeFile(reader, section, path, validateType, validateSize);
        }
        #endregion

        #region Delete
        public async Task DeleteMediaFile(string fileName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            DeleteFile(fileName, _options.BasePath);
            var storageFile = await _storageFileRepository.GetStorageFileByName(fileName, CancellationToken.None);
            await _storageFileRepository.Delete(storageFile, CancellationToken.None);
        }

        private static void DeleteFile(string fileName, string path) => FileHelper.DeleteFile(fileName, path);
        #endregion
    }
}
