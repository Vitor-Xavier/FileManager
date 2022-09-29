using FileAccess = FileManager.Models.FileAccess;

namespace FileManager.DTO
{
    public record StorageFileDto(string FileName, string Owner, FileAccess Access);
}
