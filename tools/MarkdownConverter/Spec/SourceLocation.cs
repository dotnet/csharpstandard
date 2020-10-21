using FSharp.Formatting.Common;
using FSharp.Markdown;
using Microsoft.FSharp.Core;

namespace MarkdownConverter.Spec
{
    internal class SourceLocation
    {
        public string File { get; }
        public SectionRef Section { get; }
        public MarkdownParagraph Paragraph { get; }
        public MarkdownSpan Span { get; }
        public string _loc; // generated lazily.

        public SourceLocation(string file, SectionRef section, MarkdownParagraph paragraph, MarkdownSpan span)
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
                if (_loc != null)
                {
                    return _loc;
                }

                if (File == null)
                {
                    _loc = "mdspec2docx";
                }
                else if (Section == null && Paragraph == null)
                {
                    _loc = File;
                }
                else
                {
                    // Note: we now use the F# Markdown support for ranges, rather than finding text directly.
                    // This produces slightly weaker diagnostics than before, but it avoids an awful lot of fiddly fuzzy text matching code.
                    
                    // TODO: Revisit SectionRef.Loc, possibly just exposing the paragraph directly.
                    var maybeRange = GetRange(Span);
                    if (maybeRange != null)
                    {
                        var range = maybeRange.Value;
                        _loc = $"{File}({range.StartLine},{range.StartColumn},{range.EndLine},{range.EndColumn})";
                    }
                    else
                    {
                        maybeRange = GetRange(Paragraph) ?? GetRange(Section.Loc.Paragraph);
                        if (maybeRange == null)
                        {
                            // We don't have any line or column information. Just report the filename.
                            _loc = File;
                        }
                        else
                        {
                            var range = maybeRange.Value;
                            _loc = range.StartLine == range.EndLine
                                ? $"{File}({range.StartLine})"
                                : $"{File}({range.StartLine}-{range.EndLine})";
                        }
                    }
                }

                return _loc;
            }
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
}
