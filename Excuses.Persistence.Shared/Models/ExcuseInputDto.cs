using System.Text.Json.Serialization;

namespace Excuses.Persistence.Shared.Models;

public class ExcuseInputDto
{
    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; }
}
