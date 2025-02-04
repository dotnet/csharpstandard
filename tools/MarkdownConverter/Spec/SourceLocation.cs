using System.Diagnostics.CodeAnalysis;
using FSharp.Formatting.Markdown;
using Microsoft.FSharp.Core;

namespace MarkdownConverter.Spec;

public class SourceLocation
{
    public string? File { get; }
    public SectionRef? Section { get; }
    public MarkdownParagraph? Paragraph { get; }
    public MarkdownSpan? Span { get; }
    public string? _loc; // generated lazily.

    public SourceLocation(string? file, SectionRef? section, MarkdownParagraph? paragraph, MarkdownSpan? span)
    {
        File = file;
        Section = section;
        Paragraph = paragraph;
        Span = span;
    }

    /// <summary>
    /// Description of the location, of the form "file(line/col)" in a format recognizable by msbuild.
    /// </summary>
    public string Description
    {
        get
        {
            ComputeLazyFields();
            return _loc;
        }
    }

    private int? _startLine;
    public int StartLine
    {
        get
        {
            ComputeLazyFields();
            return _startLine.Value;
        }
    }

    private int? _endLine;

    public int EndLine
    {
        get
        {
            ComputeLazyFields();
            return _endLine.Value;
        }
    }

    // Factor out this helper method to compute start line,
    // end line, and file name if any are missing.
    [MemberNotNull(nameof(_loc), nameof(_startLine), nameof(_endLine))]
    private void ComputeLazyFields()
    {
        string loc = _loc!;
        int? startLine = _startLine;
        int? endLine = _endLine;
        if (File == null)
        {
            _loc = "mdspec2docx";
            startLine = 1;
            endLine = 1;
        }
        else if (Section == null && Paragraph == null)
        {
            loc = File;
            startLine = 1;
            endLine = 1;
        }
        else
        {
            // TODO: Revisit all of the null-forgiving operator usage here at some point.

            // Note: we now use the F# Markdown support for ranges, rather than finding text directly.
            // This produces slightly weaker diagnostics than before, but it avoids an awful lot of fiddly fuzzy text matching code.

            // TODO: Revisit SectionRef.Loc, possibly just exposing the paragraph directly.
            var maybeRange = GetRange(Span!);
            if (maybeRange != null)
            {
                var range = maybeRange.Value;
                loc = $"{File}({range.StartLine},{range.StartColumn},{range.EndLine},{range.EndColumn})";
                startLine = range.StartLine;
                endLine = range.EndLine;
            }
            else
            {
                maybeRange = GetRange(Paragraph!) ?? GetRange(Section!.Loc.Paragraph!);
                if (maybeRange == null)
                {
                    // We don't have any line or column information. Just report the filename.
                    loc = File;
                    startLine = 1;
                    endLine = 1; 
                }
                else
                {
                    var range = maybeRange.Value;
                    loc = range.StartLine == range.EndLine
                        ? $"{File}({range.StartLine})"
                        : $"{File}({range.StartLine}-{range.EndLine})";
                    startLine = range.StartLine;
                    endLine = range.EndLine;
                }
            }
        }
        _loc ??= loc;
        _startLine ??= startLine;
        _endLine ??= endLine;
    }

    // Each tagged type within the F# Markdown library for pargraph/span contains a "range" property, but
    // I don't think there's a common way of getting it. So use reflection, horrible as that is...
    private static MarkdownRange? GetRange(object obj)
    {
        if (obj == null)
        {
            return null;
        }
        var rangeProperty = obj.GetType().GetProperty("range");
        var range = rangeProperty?.GetValue(obj) as FSharpOption<MarkdownRange>;
        return range?.Value;
    }
}
