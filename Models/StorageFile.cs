using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FileManager.Models
{
    public class StorageFile
    {
        public int StorageFileId { get; set; }

        public int UserId { get; set; }

        [StringLength(255)]
        public string FileName { get; set; }

        [StringLength(2000)]
        public string Path { get; set; }

        public byte[] Hash { get; set; }

        public FileAccess Access { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool Deleted { get; set; }

        [JsonIgnore]
        public User User { get; set; }
    }
}
