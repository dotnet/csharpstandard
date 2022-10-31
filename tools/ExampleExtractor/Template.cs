using Newtonsoft.Json;

namespace ExampleExtractor;

internal class Template
{
    private const string AdditionalFilesDirectory = "additional-files";
    private const string ExampleCodeSubstitution = "$example-code";
    private const string ExampleNameSubstitution = "$example-name";

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
        foreach (var pair in files)
        {
            string file = Path.Combine(outputDirectory, pair.Key);
            string code = pair.Value
                .Replace(ExampleCodeSubstitution, example.Code)
                .Replace(ExampleNameSubstitution, example.Name);
            File.WriteAllText(file, code);
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
