using System.Text.Json.Serialization;

namespace Excuses.Persistence.Shared.Models;

public class Excuse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; }
}
