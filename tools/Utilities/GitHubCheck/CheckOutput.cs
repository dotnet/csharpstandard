using System.Text.Json.Serialization;

namespace Utilities.GitHubCheck;

public class CheckOutput
{
    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("summary")]
    public required string Summary { get; set; }

    [JsonPropertyName("annotations")]
    public required List<CheckAnnotation> Annotations { get; set; }
}
