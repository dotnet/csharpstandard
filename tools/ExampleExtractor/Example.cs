using Newtonsoft.Json;

namespace ExampleExtractor;

internal class Example
{
    /// <summary>
    /// The maximum number of lines that can occur between the end of an example and the ```console
    /// line that marks the start of the expected output, when <see cref="ExampleMetadata.InferOutput"/>
    /// is true.
    /// </summary>
    private const int MaximumConsoleOutputDistance = 8;

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
            var replacements = new Queue<string>(metadata.CustomEllipsisReplacements ?? new List<string>());
            int start = 0;
            while (true)
            {
                int nextEllipsis = code.IndexOf("...", start);
                if (nextEllipsis == -1)
                {
                    break;
                }
                var replacement = replacements.TryDequeue(out var x) && x is string ? x : "/* ... */";
                code = code[0..nextEllipsis] + replacement + code[(nextEllipsis + 3)..];
                start = nextEllipsis + replacement.Length; // Move past the replacement
            }
        }
        // Remove chevrons, used for emphasis in places.
        code = code.Replace("«", "").Replace("»", "");
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

            if (metadata.InferOutput)
            {
                if (metadata.ExpectedOutput is not null)
                {
                    throw new InvalidOperationException($"Example {metadata.Name} has both ${nameof(metadata.InferOutput)} and {nameof(metadata.ExpectedOutput)}");
                }
                int openingConsoleLine = FindLineEnding(closingLine + 1, "```console");
                // We expect the output to appear very shortly after the example.
                if (openingConsoleLine > closingLine + MaximumConsoleOutputDistance)
                {
                    throw new InvalidOperationException($"Example {metadata.Name} has {nameof(metadata.InferOutput)} set but no ```console block shortly after it.");
                }
                int closingConsoleLine = FindLineEnding(openingConsoleLine, "```");
                metadata.InferOutput = false;
                metadata.ExpectedOutput = lines
                    .Skip(openingConsoleLine + 1)
                    .Take(closingConsoleLine - openingConsoleLine - 1)
                    .Select(TrimPrefix)
                    .ToList();
            }

            yield return new Example(metadata, code);
            i = closingLine;

            string TrimPrefix(string codeLine) =>
                codeLine.StartsWith(prefix) ? codeLine.Substring(prefix.Length)
                        : codeLine.StartsWith(trimmedPrefix) ? codeLine.Substring(trimmedPrefix.Length)
                        // An example may be in a list, in which case, each line starts with "  > "
                        : codeLine.StartsWith("  " + trimmedPrefix) ? codeLine.Substring(2 + trimmedPrefix.Length)
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
