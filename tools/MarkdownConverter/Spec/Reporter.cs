using FSharp.Markdown;
using System.IO;

namespace MarkdownConverter.Spec
{
    // Note: while this and SourceLocation sound like they should be somewhat neutral classes,
    // the presence of CurrentSection etc tie them closely to the Spec namespace.

    /// <summary>
    /// Diagnostic reporter
    /// </summary>
    public class Reporter
    {
        /// <summary>
        /// The text writer to write Messages to.
        /// </summary>
        private readonly TextWriter writer;

        /// <summary>
        /// The parent reporter, if any. (This is to allow a complete error/warning count to be kept.)
        /// </summary>
        private readonly Reporter parent;

        public int Errors { get; private set; }
        public int Warnings { get; private set; }

        public SourceLocation Location { get; set; } = new SourceLocation(null, null, null, null);

        private Reporter(Reporter parent, TextWriter writer, string filename)
        {
            this.parent = parent;
            this.writer = writer;
            Location = new SourceLocation(filename, null, null, null);
        }

        public Reporter(TextWriter writer) : this(null, writer, null)
        {            
        }

        public Reporter WithFileName(string filename) => new Reporter(this, writer, filename);

        public string CurrentFile => Location.File;

        public SectionRef CurrentSection
        {
            get => Location.Section;
            set => Location = new SourceLocation(CurrentFile, value, CurrentParagraph, null);
        }

        public MarkdownParagraph CurrentParagraph
        {
            get => Location.Paragraph;
            set => Location = new SourceLocation(CurrentFile, CurrentSection, value, null);
        }

        public MarkdownSpan CurrentSpan
        {
            get => Location.Span;
            set => Location = new SourceLocation(CurrentFile, CurrentSection, CurrentParagraph, value);
        }

        public void Error(string code, string msg, SourceLocation loc = null)
        {
            IncrementErrors();
            Report(code, "ERROR", msg, loc?.Description ?? Location.Description);
        }

        public void Warning(string code, string msg, SourceLocation loc = null)
        {
            IncrementWarnings();
            Report(code, "WARNING", msg, loc?.Description ?? Location.Description);
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

        public void Log(string msg) { }

        internal void Report(string code, string severity, string msg, string loc) =>
            writer.WriteLine($"{loc}: {severity} {code}: {msg}");
    }
}
