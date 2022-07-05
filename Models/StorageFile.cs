using System.ComponentModel.DataAnnotations;

namespace FileManager.Models
{
    public class StorageFile
    {
        public int StorageFileId { get; set; }

        [StringLength(30)]
        public string Owner { get; set; }

        [StringLength(255)]
        public string FileName { get; set; }

        [StringLength(2000)]
        public string Path { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool Deleted { get; set; }
    }
}
