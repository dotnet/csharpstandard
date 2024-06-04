using System.Text.Json.Serialization;

namespace Utilities.GitHubCheck;

[JsonConverter(typeof(JsonStringEnumConverter<AnnotationLevel>))]
public enum AnnotationLevel
{
    Notice,
    Warning,
    Failure
}

public readonly record struct CheckAnnotation
{
    public string Path { get; init; }

    public string StartLine { get; init; }

    public string EndLine { get; init; }

    public AnnotationLevel Level { get; init; }

    public string Message { get; init; }
}
