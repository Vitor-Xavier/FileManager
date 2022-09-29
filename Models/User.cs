using System.Text.Json.Serialization;

namespace FileManager.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool Deleted { get; set; }

        [JsonIgnore]
        public virtual ICollection<StorageFile> StorageFiles { get; set; }
    }
}
