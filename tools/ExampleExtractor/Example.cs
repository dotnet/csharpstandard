using Newtonsoft.Json;
using System.Net.Http.Json;

namespace ExampleExtractor;

internal class Example
{
    private const string ExampleCommentPrefix = "<!-- Example: ";
    private const string CommentSuffix = " -->";

    internal ExampleMetadata Metadata { get; }

    /// <summary>
    /// The name of the example. This should be unique across all files.
    /// </summary>
    internal string Name => Metadata.Name;

    /// <summary>
    /// The name of the template to apply.
    /// </summary>
    internal string Template => Metadata.Template;

    /// <summary>
    /// The source location of the example.
    /// </summary>
    internal string Source => Metadata.Source;

    /// <summary>
    /// The code within the example.
    /// </summary>
    internal string Code { get; }

    /// <summary>
    /// Loads examples from all the Markdown files in the given directory.
    /// </summary>
    internal static List<Example> LoadExamplesFromDirectory(string directory) =>
        Directory.GetFiles(directory, "*.md")
            .SelectMany(LoadExamplesFromFile)
            .ToList();

    private Example(ExampleMetadata metadata, string code)
    {
        Metadata = metadata;
        if (metadata.ReplaceEllipsis)
        {
            code = code.Replace("...", "/* ... */");
        }
        Code = code;
    }

    /// <summary>
    /// Loads examples from a single Markdown file.
    /// </summary>
    private static IEnumerable<Example> LoadExamplesFromFile(string markdownFile)
    {
        string[] lines = File.ReadAllLines(markdownFile);

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (!line.Contains(ExampleCommentPrefix))
            {
                continue;
            }
            var metadata = ParseComment(line);

            string prefix = line.Substring(0, line.IndexOf(ExampleCommentPrefix));
            string trimmedPrefix = prefix.Trim();

            // We don't currently assume the example comes immediately after the comment.
            // This could allow for expected output in another comment, for example.
            // If it turns out not to be useful, we could just check that lines[i+1] ends with ```csharp
            int openingLine = FindLineEnding(i, "```csharp"); // 0-based, and pre-code
            int closingLine = FindLineEnding(openingLine, "```"); // 0-based, and post-code

            var codeLines = lines
                .Skip(openingLine + 1)
                .Take(closingLine - openingLine - 1)
                .Select(TrimPrefix);

            string code = string.Join("\n", codeLines);

            // Augment the metadata
            metadata.StartLine = openingLine + 1;
            metadata.EndLine = closingLine;
            metadata.MarkdownFile = Path.GetFileName(markdownFile);

            yield return new Example(metadata, code);
            i = closingLine;

            string TrimPrefix(string codeLine) =>
                codeLine.StartsWith(prefix) ? codeLine.Substring(prefix.Length)
                        : codeLine.StartsWith(trimmedPrefix) ? codeLine.Substring(trimmedPrefix.Length)
                        : throw new InvalidOperationException($"Example in {markdownFile} starting at line {openingLine} contains line without common prefix");

        }

        int FindLineEnding(int start, string suffix)
        {
            for (int i = start; i < lines.Length; i++)
            {
                if (lines[i].EndsWith(suffix))
                {
                    return i;
                }
            }
            throw new InvalidOperationException($"File {markdownFile} has no line ending '{suffix}' starting at line {start + 1}");
        }

        ExampleMetadata ParseComment(string commentLine)
        {
            int prefixIndex = commentLine.IndexOf(ExampleCommentPrefix);
            if (prefixIndex == -1)
            {
                throw new ArgumentException($"'{commentLine}' does not contain {ExampleCommentPrefix}");
            }
            if (!commentLine.EndsWith(CommentSuffix))
            {
                throw new ArgumentException($"'{commentLine}' does not end with {CommentSuffix}");
            }
            string json = commentLine[(prefixIndex + ExampleCommentPrefix.Length)..^CommentSuffix.Length];
            try
            {
                return JsonConvert.DeserializeObject<ExampleMetadata>(json) ?? throw new ArgumentException("Invalid (null) configuration");
            }
            catch (JsonException e)
            {
                // TODO: Add the source information as well.
                throw new Exception($"Error parsing metadata '{json}'", e);
            }
        }
    }
}
