using System.Text.Json;

namespace Utilities;

/// <summary>
/// Record for a single diagnostic
/// </summary>
/// <param name="file">The source file in the PR</param>
/// <param name="Message">The message for the output daignostic</param>
/// <param name="Id">The error message ID</param>
/// <param name="StartLine">The start line (index from 1)</param>
/// <param name="EndLine">The end line (index from 1)</param>
public record StatusCheckMessage(string file, int StartLine, int EndLine, string Message, string Id);

/// <summary>
/// A tool-neutral diagnostic with a repository-relative file path and raw message.
/// </summary>
public record StatusCheckDiagnostic(
    string Severity, string File, int StartLine, int EndLine, string Id, string Message);

/// <summary>
/// Collects tool diagnostics and writes console output, workflow commands, and JSON reports.
/// </summary>
/// <remarks>
/// In GitHub Actions, diagnostics produce runner-generated annotations. GitHub may also
/// display them inline when their source locations match the PR diff.
/// </remarks>
/// <param name="pathToRoot">The path to the root of the repository</param>
/// <param name="toolName">The name of the tool that is running the check</param>
public class StatusCheckLogger(TextWriter writer, string pathToRoot, string toolName)
{
    private readonly List<StatusCheckDiagnostic> diagnostics = [];
    public IReadOnlyList<StatusCheckDiagnostic> Diagnostics => diagnostics.AsReadOnly();
    public bool Success { get; private set; } = true;

    // Utility method to format the path to unix style, from the root of the repository.
    private string FormatPath(string path) =>
        Path.GetRelativePath(
            pathToRoot.Replace('\\', Path.DirectorySeparatorChar),
            path.Replace('\\', Path.DirectorySeparatorChar)).Replace('\\', '/');

    private void WriteMessageToConsole(string prefix, StatusCheckMessage d) => writer.WriteLine($"{prefix}{toolName}-{d.Id}::file={FormatPath(d.file)},line={d.StartLine}::{d.Message}");

    private void LogDiagnostic(string prefix, string severity, StatusCheckMessage d)
    {
        WriteMessageToConsole(prefix, d);
        var diagnostic = new StatusCheckDiagnostic(
            severity, FormatPath(d.file), d.StartLine, d.EndLine, d.Id, d.Message);
        diagnostics.Add(diagnostic);

        if (string.Equals(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"),
            "true", StringComparison.OrdinalIgnoreCase))
        {
            writer.WriteLine(FormatWorkflowCommand(diagnostic));
        }
    }

    private string FormatWorkflowCommand(StatusCheckDiagnostic d)
    {
        List<string> properties = [$"file={EscapeProperty(d.File)}"];
        if (d.StartLine > 0)
        {
            properties.Add($"line={d.StartLine}");
            if (d.EndLine >= d.StartLine)
            {
                properties.Add($"endLine={d.EndLine}");
            }
        }
        properties.Add($"title={EscapeProperty($"{toolName} - {d.Id}")}");
        return $"::{d.Severity} {string.Join(",", properties)}::{EscapeData(d.Message)}";
    }

    private static string EscapeData(string value) =>
        value.Replace("%", "%25").Replace("\r", "%0D").Replace("\n", "%0A");

    private static string EscapeProperty(string value) =>
        EscapeData(value).Replace(":", "%3A").Replace(",", "%2C");

    /// <summary>
    /// Log a notice from the status check to the console only
    /// </summary>
    /// <param name="source">source file</param>
    /// <param name="startLine">start line</param>
    /// <param name="endLine">end line</param>
    /// <param name="message">The error message</param>
    /// <param name="id">The error ID</param>
    /// <remarks>
    /// Log the diagnostic information to console. These will only appear in the console window, not
    /// as annotations on the changes in the PR. 
    /// </remarks>
    public void ConsoleOnlyLog(string source, int startLine, int endLine, string message, string id) =>
        ConsoleOnlyLog(source, new StatusCheckMessage(source, startLine, endLine, message, id));

    /// <summary>
    /// Log a notice from the status check to the console only
    /// </summary>
    /// <param name="d">The diagnostic</param>
    /// <remarks>
    /// Log the diagnostic information to console. These will only appear in the console window, not
    /// as annotations on the changes in the PR. 
    /// </remarks>
    public void ConsoleOnlyLog(string source, StatusCheckMessage d) =>
        WriteMessageToConsole("", d);

    /// <summary>
    /// Log a notice from the status check
    /// </summary>
    /// <param name="d">The diagnostic</param>
    /// <remarks>
    /// Collect the diagnostic and
    /// log the diagnostic information to console.
    /// </remarks>
    public void LogNotice(StatusCheckMessage d) => LogDiagnostic("", "notice", d);

    /// <summary>
    /// Log a warning from the status check
    /// </summary>
    /// <param name="d">The diagnostic</param>
    /// <remarks>
    /// Collect the diagnostic and
    /// log the warning notice to the console.
    /// Warnings are logged, but the process reports "success" to GitHub.
    /// </remarks>
    public void LogWarning(StatusCheckMessage d) => LogDiagnostic("⚠️", "warning", d);

    /// <summary>
    /// Log a failure from the status check
    /// </summary>
    /// <param name="source">The source file</param>
    /// <param name="startLine">Start line in source</param>
    /// <param name="endLine">End line in source</param>
    /// <param name="message">The error message</param>
    /// <param name="id">The string ID for the error</param>
    public void LogFailure(string source, int startLine, int endLine, string message, string id) =>
        LogFailure(new(source, startLine, endLine, message, id));


    /// <summary>
    /// Log a failure from the status check
    /// </summary>
    /// <param name="d">The diagnostic</param>
    /// <remarks>
    /// Collect the diagnostic and
    /// log the failure notice to the console.
    /// This method is distinct from <see cref="ExitOnFailure(StatusCheckMessage)"/> in
    /// that this method does not throw an exception. Its purpose is to log
    /// the failure but allow the tool to continue running further checks.
    /// </remarks>
    public void LogFailure(StatusCheckMessage d)
    {
        LogDiagnostic("❌", "error", d);
        Success = false;
    }

    /// <summary>
    /// Log a failure from the status check and throw for exit
    /// </summary>
    /// <param name="d">The diagnostic</param>
    /// <remarks>
    /// Collect the diagnostic and
    /// log the failure notice to the console.
    /// This method is distinct from <see cref="LogFailure(StatusCheckMessage)"/> in
    /// that this method throws an exception. Its purpose is to log
    /// the failure and immediately exit, foregoing any further checks.
    /// </remarks>
    public void ExitOnFailure(StatusCheckMessage d)
    {
        LogFailure(d);
        throw new InvalidOperationException(d.Message);
    }

    /// <summary>
    /// Write the complete diagnostic report, creating its parent directory.
    /// </summary>
    /// <param name="outputPath">The destination JSON file.</param>
    /// <param name="completedSuccessfully">
    /// Whether the tool completed successfully, including failures outside diagnostic logging.
    /// </param>
    public async Task WriteReportAsync(string outputPath, bool completedSuccessfully = true)
    {
        var fullPath = Path.GetFullPath(outputPath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        await using var stream = File.Create(fullPath);
        await JsonSerializer.SerializeAsync(stream, new
        {
            tool = toolName,
            success = Success && completedSuccessfully,
            diagnostics
        }, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });
    }
}
