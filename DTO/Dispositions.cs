using System.Text.Json.Serialization;

namespace FileManager.DTO
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Dispositions
    {
        None,
        Attachment,
        Inline
    }
}
