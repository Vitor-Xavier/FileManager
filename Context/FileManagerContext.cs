using FileManager.Models;
using Microsoft.EntityFrameworkCore;

namespace FileManager.Context
{
    public class FileManagerContext : DbContext
    {
        public FileManagerContext(DbContextOptions<FileManagerContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<StorageFile> StorageFiles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;ConnectRetryCount=0");
        }
    }
}
