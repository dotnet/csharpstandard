using FSharp.Formatting.Markdown;
using Utilities;

namespace MarkdownConverter.Spec;

// Note: while this and SourceLocation sound like they should be somewhat neutral classes,
// the presence of CurrentSection etc tie them closely to the Spec namespace.

/// <summary>
/// Diagnostic reporter
/// </summary>
public class Reporter
{
    private readonly StatusCheckLogger githubLogger;

    /// <summary>
    /// The parent reporter, if any. (This is to allow a complete error/warning count to be kept.)
    /// </summary>
    private readonly Reporter? parent;

    public int Errors { get; private set; }
    public int Warnings { get; private set; }

    public SourceLocation Location { get; set; } = new SourceLocation(null, null, null, null);

    public Reporter() : this(null, null) { }

    public Reporter(Reporter? parent, string? filename)
    {
        // This is needed so that all Reporters share the same GitHub logger.
        this.githubLogger = parent?.githubLogger ?? new StatusCheckLogger("..", "Markdown to Word Converter");
        this.parent = parent;
        Location = new SourceLocation(filename, null, null, null);
    }

    public Reporter WithFileName(string filename) => new Reporter(this, filename);

    public string? CurrentFile => Location.File;

    public SectionRef? CurrentSection
    {
        get => Location.Section;
        set => Location = new SourceLocation(CurrentFile, value, CurrentParagraph, null);
    }

    public MarkdownParagraph? CurrentParagraph
    {
        get => Location.Paragraph;
        set => Location = new SourceLocation(CurrentFile, CurrentSection, value, null);
    }

    public MarkdownSpan? CurrentSpan
    {
        get => Location.Span;
        set => Location = new SourceLocation(CurrentFile, CurrentSection, CurrentParagraph, value);
    }

    public void Error(string code, string msg, SourceLocation? loc = null)
    {
        loc = loc ?? Location;
        IncrementErrors();
        githubLogger.LogFailure(new StatusCheckMessage(loc.File ?? "mdspec2docx", loc.StartLine, loc.EndLine, msg, code));
    }

    public void Warning(string code, string msg, SourceLocation? loc = null, int lineOffset = 0)
    {
        loc = loc ?? Location;
        IncrementWarnings();
        githubLogger.LogWarning(new StatusCheckMessage(loc.File ?? "mdspec2docx", loc.StartLine+lineOffset, loc.EndLine+lineOffset, msg, code));
    }

    public void Log(string code, string msg, SourceLocation? loc = null)
    {
        loc = loc ?? Location;
        // githubLogger.LogNotice(new Diagnostic(loc.File ?? "mdspec2docx", loc.StartLine, loc.EndLine, msg, code));
    }

    private void IncrementWarnings()
    {
        Warnings++;
        parent?.IncrementWarnings();
    }

    private void IncrementErrors()
    {
        Errors++;
        parent?.IncrementErrors();
    }

    internal async Task WriteCheckStatus(string token, string head_sha) =>
        await githubLogger.BuildCheckRunResult(token, "dotnet", "csharpstandard", head_sha);
}
