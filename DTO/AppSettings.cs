namespace FileManager.DTO
{
    public record AppSettings
    {
        public string Secret { get; set; }

        public int ExpiresIn { get; set; }
    }
}
