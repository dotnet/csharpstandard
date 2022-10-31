using ExampleExtractor;

if (args.Length != 3)
{
    Console.WriteLine("Arguments: <markdown-directory> <template-directory> <output-directory>");
    return 1;
}

string markdownDirectory = args[0];
string templateDirectory = args[1];
string outputDirectory = args[2];

if (!ValidateDirectory(markdownDirectory) ||
    !ValidateDirectory(templateDirectory))
{
    return 1;
}

if (Directory.Exists(outputDirectory))
{
    if (Directory.GetFiles(outputDirectory).Any())
    {
        Console.WriteLine($"Error: {outputDirectory} exists and contains files.");
    }
    var oldSubdirectories = Directory.GetDirectories(outputDirectory);
    if (oldSubdirectories.Any())
    {
        Console.WriteLine($"Deleting old output subdirectories ({oldSubdirectories.Length})");
        foreach (var subdirectory in oldSubdirectories)
        {
            Directory.Delete(subdirectory, true);
        }
    }
}
else
{
    Directory.CreateDirectory(outputDirectory);
}

var templates = Template.LoadTemplates(templateDirectory);
Console.WriteLine($"Loaded {templates.Count} templates");
var examples = Example.LoadExamplesFromDirectory(markdownDirectory);
Console.WriteLine($"Loaded {examples.Count} examples");
var duplicates = examples.GroupBy(x => x.Name).Where(g => g.Count() > 1).ToList();
if (duplicates.Any())
{
    Console.WriteLine("The following examples have duplicate names:");
    foreach (var group in duplicates)
    {
        Console.WriteLine($"  {group.Key}:");
        foreach (var item in group)
        {
            Console.WriteLine($"    {item.Source}");
        }
    }
    return 1;
}

bool anyErrors = false;
foreach (var group in examples.GroupBy(x => x.Metadata.MarkdownFile))
{
    Console.WriteLine($"Processing {group.Count()} examples from {group.Key}");
    foreach (var example in group)
    {
        if (!templates.TryGetValue(example.Template, out var template))
        {
            Console.WriteLine($"ERROR: template '{example.Template}' not found for {example.Name}");
            anyErrors = true;
            continue;
        }
        template.Apply(example, outputDirectory, templateDirectory);
    }
}
Console.WriteLine("Finished example extraction.");

return anyErrors ? 1 : 0;

bool ValidateDirectory(string directory)
{
    if (!Directory.Exists(directory))
    {
        Console.WriteLine($"Error: '{directory}' does not exist or is not a directory");
        return false;
    }
    return true;
}