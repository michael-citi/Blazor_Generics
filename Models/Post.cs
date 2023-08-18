using System.Text.Json.Serialization;

namespace ComponentTest.Models;

public class Post
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("content")]
    public string? Content { get; set; }
    
    [JsonPropertyName("dateCreated")]
    public DateTime DateCreated { get; set; }
    
    [JsonPropertyName("lastModified")]
    public DateTime LastModified { get; set; }
}