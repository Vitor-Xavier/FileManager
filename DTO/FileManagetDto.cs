namespace FileManager.DTO
{
    public record FileManagerDto(string FileName, string FilePath, string ContentType);

    public record FileManagerPostDto(string FileName, string FilePath, string ContentType, byte[] Hash);
}
