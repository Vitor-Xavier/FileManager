using FileManager.Context;
using FileManager.Models;
using Microsoft.EntityFrameworkCore;

namespace FileManager.Repositories.StorageFiles
{
    public class StorageFileRepository : Repository<StorageFile, FileManagerContext>, IStorageFileRepository
    {
        public StorageFileRepository(FileManagerContext context) : base(context) { }

        public Task<StorageFile> GetStorageFileByName(string fileName, CancellationToken cancellationToken) =>
            _context.StorageFiles.Where(s => s.FileName == fileName).FirstOrDefaultAsync(cancellationToken);
    }
}
