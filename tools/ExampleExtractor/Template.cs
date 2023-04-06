using Newtonsoft.Json;

namespace ExampleExtractor;

internal class Template
{
    private const string AdditionalFilesDirectory = "additional-files";
    private const string ExampleCodeSubstitution = "$example-code";
    private const string ExampleNameSubstitution = "$example-name";
    private const string FileCommentPrefix = "// File ";

    internal string Name { get; }
    private readonly Dictionary<string, string> files;

    private Template(string name, Dictionary<string, string> files)
    {
        Name = name;
        this.files = files;
    }

    /// <summary>
    /// Applies the given example to this template, writing it out to an output directory.
    /// </summary>
    /// <param name="example">The example to apply.</param>
    /// <param name="rootOutputDirectory">The root output directory. (A subdirectory for the example will be created within this.)</param>
    /// <param name="rootTemplateDirectory">The root template directory, used for finding additional files if necessary.</param>
    internal void Apply(Example example, string rootOutputDirectory, string rootTemplateDirectory)
    {
        var outputDirectory = Path.Combine(rootOutputDirectory, example.Name);
        Directory.CreateDirectory(outputDirectory);

        var code = ExtractExtraFiles(example.Code, outputDirectory);

        foreach (var pair in files)
        {
            string file = Path.Combine(outputDirectory, pair.Key);
            string content = pair.Value
                .Replace(ExampleCodeSubstitution, code)
                .Replace(ExampleNameSubstitution, example.Name);
            File.WriteAllText(file, content);
        }
        if (example.Metadata.AdditionalFiles is List<string> additionalFiles)
        {
            foreach (var additionalFile in additionalFiles)
            {
                string sourceFile = Path.Combine(rootTemplateDirectory, AdditionalFilesDirectory, additionalFile);
                string destFile = Path.Combine(outputDirectory, additionalFile);
                File.Copy(sourceFile, destFile);
            }
        }
        var metadataJson = JsonConvert.SerializeObject(example.Metadata);
        File.WriteAllText(Path.Combine(outputDirectory, ExampleMetadata.MetadataFile), metadataJson);
    }

    /// <summary>
    /// Returns all the code before the first "// File:" comment, extracting any additional
    /// files into the given directory.
    /// </summary>
    private string ExtractExtraFiles(string code, string outputDirectory)
    {
        var lines = code.Split('\n').ToList();
        // The implementation is a lot simpler if we know we've got an end marker.
        lines.Add(FileCommentPrefix + "IgnoreMe");
        string? currentFile = null;
        List<string> currentLines = new List<string>();
        string? initialCode = null;

        foreach (var line in lines)
        {
            if (line.StartsWith(FileCommentPrefix))
            {
                if (currentFile is null)
                {
                    // Remember this for later.
                    initialCode = string.Join("\n", currentLines);
                }
                else
                {
                    File.WriteAllLines(currentFile, currentLines);
                }
                currentLines.Clear();
                currentFile = Path.Combine(outputDirectory, line[FileCommentPrefix.Length..].TrimEnd(':'));
            }
            else
            {
                currentLines.Add(line);
            }
        }
        return initialCode ?? throw new InvalidOperationException($"Never saw a file terminator - bug in {nameof(ExtractExtraFiles)}");
    }

    internal static Dictionary<string, Template> LoadTemplates(string directory) =>
        Directory.GetDirectories(directory)
            .Select(LoadTemplate)
            .ToDictionary(template => template.Name);

    private static Template LoadTemplate(string directory)
    {
        var name = Path.GetFileName(directory);
        var files = Directory.GetFiles(directory)
            .Where(file => Path.GetFileName(file) != AdditionalFilesDirectory)
            .ToDictionary(file => Path.GetFileName(file), file => File.ReadAllText(file));
        return new Template(name, files);
    }
}
