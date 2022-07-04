using FileManager.Models;

namespace FileManager.Repositories.StorageFiles
{
    public interface IStorageFileRepository : IRepository<StorageFile>
    {
        Task<StorageFile> GetStorageFileByName(string fileName, CancellationToken cancellationToken);
    }
}
