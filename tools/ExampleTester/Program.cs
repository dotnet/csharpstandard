using ExampleTester;
using System.CommandLine;

var rootCommand = new RootCommand();
new TesterConfigurationBinder().ConfigureCommand(rootCommand, ExecuteAsync);
return rootCommand.Invoke(args);

async Task<int> ExecuteAsync(TesterConfiguration configuration)
{
    var parentDirectory = configuration.ExtractedOutputDirectory;

    if (!Directory.Exists(parentDirectory))
    {
        Console.WriteLine($"Error: '{parentDirectory}' does not exist or is not a directory");
        return 1;
    }

    int failures = 0;
    var examples = GeneratedExample.LoadAllExamples(parentDirectory)
        .OrderBy(e => e.Metadata.MarkdownFile).ThenBy(e => e.Metadata.StartLine)
        .Where(e => configuration.SourceFile is not string sourceFile || e.Metadata.MarkdownFile == sourceFile)
        .Where(e => configuration.ExampleName is not string exampleName || e.Metadata.Name == exampleName)
        .ToList();

    if (examples.Count == 0)
    {
        Console.WriteLine("Error: no examples to run. Check source/example options.");
        return 1;
    }

    foreach (var example in examples)
    {
        // The Run method explains any failures, we just need to count them.
        if (!await example.Test(configuration))
        {
            failures++;
        }
    }

    Console.WriteLine();
    Console.WriteLine($"Tests: {examples.Count}");
    Console.WriteLine($"Failures: {failures}");

    return failures;
}