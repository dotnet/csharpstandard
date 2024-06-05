﻿using Utilities.GitHubCheck;

namespace Utilities;

/// <summary>
/// Record for a single diagnostic
/// </summary>
/// <param name="file">The source file in the PR</param>
/// <param name="Message">The message for the output daignostic</param>
/// <param name="Id">The error message ID</param>
/// <param name="StartLine">The start line (index from 1)</param>
/// <param name="EndLine">The end line (index from 1)</param>
public record Diagnostic(string file, int StartLine, int EndLine, string Message, string Id);

/// <summary>
/// This class writes the status of the check to the console in the format GitHub supports
/// </summary>
/// <remarks>
/// For all of our tools, if all error and warning messages are formatted correctly, GitHub
/// will show those errors and warnings inline in the files tab for the PR. Let's format
/// them correctly.
/// </remarks>
/// <param name="pathToRoot">The path to the root of the repository</param>
/// <param name="toolName">The name of the tool that is running the check</param>
public class StatusCheckLogger(string pathToRoot, string toolName)
{
    private List<CheckAnnotation> annotations = [];
    public bool Success { get; private set; } = true;

    // Utility method to format the path to unix style, from the root of the repository.
    private string FormatPath(string path) => Path.GetRelativePath(pathToRoot, path).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

    private void WriteMessageToConsole(string prefix, Diagnostic d) => Console.WriteLine($"{prefix}{toolName}-{d.Id}::file={FormatPath(d.file)},line={d.StartLine}::{d.Message}");

    /// <summary>
    /// Log a notice from the status check
    /// </summary>
    /// <param name="d">The diagnostic</param>
    /// <remarks>
    /// Add the diagnostic to the annotation list and
    /// log the diagnostic information to console.
    /// </remarks>
    public void LogNotice(Diagnostic d)
    {
        WriteMessageToConsole("", d);
        annotations.Add(new ()
        {
            Path = FormatPath(d.file),
            StartLine = d.StartLine.ToString(),
            EndLine = d.EndLine.ToString(),
            Level = AnnotationLevel.Notice,
            Message = d.Message
        });
    }

    /// <summary>
    /// Log a warning from the status check
    /// </summary>
    /// <param name="d">The diagnostic</param>
    /// <remarks>
    /// Add the diagnostic to the annotation list and
    /// log the warning notice to the console.
    /// </remarks>
    public void LogWarning(Diagnostic d)
    {
        WriteMessageToConsole("⚠️", d);
        annotations.Add(new ()
        {
            Path = FormatPath(d.file),
            StartLine = d.StartLine.ToString(),
            EndLine = d.EndLine.ToString(),
            Level = AnnotationLevel.Warning,
            Message = d.Message
        });
        Success = false;
    }

    /// <summary>
    /// Log a failure from the status check
    /// </summary>
    /// <param name="d">The diagnostic</param>
    /// <remarks>
    /// Add the diagnostic to the annotation list and
    /// log the failure notice to the console.
    /// This method is distinct from <see cref="ExitOnFailure(Diagnostic)"/> in
    /// that this method does not throw an exception. Its purpose is to log
    /// the failure but allow the tool to continue running further checks.
    /// </remarks>
    public void LogFailure(Diagnostic d)
    {
        WriteMessageToConsole("❌", d);
        annotations.Add(new()
        {
            Path = FormatPath(d.file),
            StartLine = d.StartLine.ToString(),
            EndLine = d.EndLine.ToString(),
            Level = AnnotationLevel.Failure,
            Message = d.Message
        });
        Success = false;
    }

    /// <summary>
    /// Log a failure from the status check and throw for exit
    /// </summary>
    /// <param name="d">The diagnostic</param>
    /// <remarks>
    /// Add the diagnostic to the annotation list and
    /// log the failure notice to the console.
    /// This method is distinct from <see cref="LogFailure(Diagnostic)"/> in
    /// that this method throws an exception. Its purpose is to log
    /// the failure and immediately exit, foregoing any further checks.
    /// </remarks>
    public void ExitOnFailure(Diagnostic d)
    {
        LogFailure(d);
        throw new InvalidOperationException(d.Message);
    }

    /// <summary>
    /// Return the object required for a full status check
    /// </summary>
    /// <param name="owner">The GitHub owner (or organization)</param>
    /// <param name="repo">The GitHub repo name</param>
    /// <param name="sha">The head sha when running as a GitHub action</param>
    /// <returns>The full check run result object</returns>
    public CheckResult BuildCheckRunResult(string owner, string repo, string sha)
    {
        return new ()
        {
            Owner = owner,
            Repo = repo,
            HeadSha = sha,
            Name = toolName,
            Status = CheckStatus.Completed,
            Conclusion = Success ? CheckConclusion.Success : CheckConclusion.Failure,
            Output = new()
            {
                Title = $"{toolName} Check Run results",
                Summary = $"{toolName} result is {(Success ? "success" : "failure")} with {annotations.Count} diagnostics.",
                Annotations = annotations
            }
        };
    }
}
