using Newtonsoft.Json;

#nullable disable

namespace ExampleExtractor;

/// <summary>
/// Metadata about an example, from the configuration in the Markdown comment
/// </summary>
public class ExampleMetadata
{
    public const string MetadataFile = "metadata.json";

    // Information loaded from the comment
    [JsonProperty("template")]
    public string Template { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("replaceEllipsis")]
    public bool ReplaceEllipsis { get; set; }

    /// <summary>
    /// When null, the sample directory is expected to contain a single .csproj file.
    /// Specify this as the project file (without extension) to disambiguate for
    /// samples with multiple project files.
    /// </summary>
    [JsonProperty("project")]
    public string Project { get; set; }

    /// <summary>
    /// When set, and when <see cref="ReplaceEllipsis"/> is true, this list is used
    /// to provide the replacements, allowing for return statements etc.
    /// A null entry is used to mean the default "/* ... */" replacement. (The default
    /// is also used for anything beyond the length of this list.)
    /// </summary>
    [JsonProperty("customEllipsisReplacements")]
    public List<string> CustomEllipsisReplacements { get; set; }

    [JsonProperty("expectedErrors")]
    public List<string> ExpectedErrors { get; set; }
    [JsonProperty("expectedWarnings")]
    public List<string> ExpectedWarnings { get; set; }
    [JsonProperty("ignoredWarnings")]
    public List<string> IgnoredWarnings { get; set; }
    [JsonProperty("expectedOutput")]
    public List<string> ExpectedOutput { get; set; }

    /// <summary>
    /// If this is set, ExpectedOutput must be null. The expected
    /// output is inferred by finding a console output section shortly after the example.
    /// This is always false in the metadata after extraction: the inferred output
    /// is placed in ExpectedOutput instead.
    /// </summary>
    [JsonProperty("inferOutput")]
    public bool InferOutput { get; set; }

    /// <summary>
    /// If this is set, ExpectedOutput must be null and InferOutput must be false.
    /// The actual output is then ignored by the test runner.
    /// This option should be used when output is nondeterministic.
    /// </summary>
    [JsonProperty("ignoreOutput")]
    public bool IgnoreOutput { get; set; }

    [JsonProperty("expectedException")]
    public string ExpectedException { get; set; }

    [JsonProperty("executionArgs")]
    public string[] ExecutionArgs { get; set; }

    /// <summary>
    /// Additional files to copy from the special "additional-files" template directory.
    /// </summary>
    [JsonProperty("additionalFiles")]
    public List<string> AdditionalFiles { get; set; }

    // Information provided by the example extractor
    [JsonProperty("markdownFile")]
    public string MarkdownFile { get; set; }
    [JsonProperty("startLine")]
    public int StartLine { get; set; }
    [JsonProperty("endLine")]
    public int EndLine { get; set; }

    [JsonIgnore]
    public string Source => $"{MarkdownFile}:{StartLine}-{EndLine}";
}
