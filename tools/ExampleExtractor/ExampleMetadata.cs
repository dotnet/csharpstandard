using System.Text.Json.Serialization;

#nullable disable

namespace ExampleExtractor;

/// <summary>
/// Metadata about an example, from the configuration in the Markdown comment
/// </summary>
public class ExampleMetadata
{
    public const string MetadataFile = "metadata.json";

    // Information loaded from the comment
    public string Template { get; set; }
    public string Name { get; set; }
    public bool ReplaceEllipsis { get; set; }
    public List<string> ExpectedErrors { get; set; }
    public List<string> ExpectedWarnings { get; set; }
    public List<string> IgnoredWarnings { get; set; }
    public List<string> ExpectedOutput { get; set; }
    /// <summary>
    /// If this is set, ExpectedOutput must be null. The expected
    /// output is inferred by finding a console output section shortly after the example.
    /// This is always false in the metadata after extraction: the inferred output
    /// is placed in ExpectedOutput instead.
    /// </summary>
    public bool InferOutput { get; set; }
    public string ExpectedException { get; set; }

    // Information provided by the example extractor
    public string MarkdownFile { get; set; }
    public int StartLine { get; set; }
    public int EndLine { get; set; }

    [JsonIgnore]
    public string Source => $"{MarkdownFile}:{StartLine}-{EndLine}";
}
