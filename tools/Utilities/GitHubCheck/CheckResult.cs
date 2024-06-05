using System.Text.Json;
using System.Text.Json.Serialization;

namespace Utilities.GitHubCheck;

[JsonConverter(typeof(JsonStringEnumConverter<CheckStatus>))]
public enum CheckStatus
{
    Queued,
    InProgress,
    Requested,
    Waiting,
    Pending,
    Completed
}

[JsonConverter(typeof(JsonStringEnumConverter<CheckConclusion>))]
public enum CheckConclusion
{
    ActionRequired,
    Cancelled,
    Failure,
    Neutral,
    Skipped,
    Stale,
    StartupFailure,
    Success
}

public record class CheckResult
{
    public required string Owner { get; init; }

    public required string Repo { get; init; }

    public required string Name { get; init; }

    public required string HeadSha { get; init; }

    public required CheckStatus Status { get; init; }

    public required CheckConclusion Conclusion { get; init; }

    public required CheckOutput Output { get; init; }

    public string ToJson() => JsonSerializer.Serialize(this,
        new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) },
            WriteIndented = true
        });
}
